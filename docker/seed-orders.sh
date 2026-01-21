
#!/usr/bin/env bash
set -euo pipefail

BOOTSTRAP="${BOOTSTRAP_SERVERS:-kafka:29092}"   # internal listener for containers
TOPIC="${TOPIC_ORDERS:-orders}"
COUNT="${SEED_COUNT:-100}"
INTERVIEW_MODE="${INTERVIEW_MODE:-true}"  # Use interview-orders.txt by default

# Product catalog: id|name|price_in_cents
CATALOG=(
  "P-001|Keyboard|4990"
  "P-002|Mouse|2450"
  "P-003|Monitor 24in|14900"
  "P-004|Laptop Stand|3200"
  "P-005|Dock|8900"
  "P-006|Headset|5990"
  "P-007|HDMI Cable|750"
  "P-008|USB-C Cable|850"
  "P-009|SSD 1TB|8990"
  "P-010|Webcam|3900"
)

# random integer in [min, max]
rand_between() {
  local min=$1 max=$2
  echo $(( RANDOM % (max - min + 1) + min ))
}

# escape quotes and backslashes (simple)
json_escape() {
  echo "$1" | sed 's/\\/\\\\/g; s/"/\\"/g'
}

# format cents -> "X.YY"
fmt_cents() {
  local cents=$1
  local whole=$(( cents / 100 ))
  local frac=$(( cents % 100 ))
  printf "%d.%02d" "$whole" "$frac"
}

generate_order_json() {
  local idx="$1"

  local orderId
  orderId=$(printf "ORD-%06d" "$idx")

  local now
  # UTC ISO 8601
  now=$(date -u +"%Y-%m-%dT%H:%M:%SZ")

  local items
  items=$(rand_between 1 4)

  local total_cents=0
  local arr="["
  for ((i=0; i<items; i++)); do
    # pick a random product
    local pick=${CATALOG[$(rand_between 0 $((${#CATALOG[@]} - 1)))]}
    local pid=$(echo "$pick" | cut -d'|' -f1)
    local name=$(echo "$pick" | cut -d'|' -f2)
    local price_cents=$(echo "$pick" | cut -d'|' -f3)

    local amount
    amount=$(rand_between 1 3)

    local subtotal=$(( price_cents * amount ))
    total_cents=$(( total_cents + subtotal ))

    local name_esc
    name_esc=$(json_escape "$name")

    # format product unit price from cents to X.YY
    local price_fmt
    price_fmt=$(fmt_cents "$price_cents")

    arr+="{\"productId\":\"$pid\",\"name\":\"$name_esc\",\"value\":$price_fmt,\"amount\":$amount}"
    if [[ $i -lt $((items-1)) ]]; then
      arr+=","
    fi
  done
  arr+="]"

  local total_fmt
  total_fmt=$(fmt_cents "$total_cents")

  # Emit compact JSON object
  echo -n "{\"orderId\":\"$orderId\",\"time\":\"$now\",\"products\":$arr,\"value\":$total_fmt}"
}

# Check if interview mode is enabled
if [ "$INTERVIEW_MODE" = "true" ] && [ -f /scripts/interview-orders.txt ]; then
  echo "[seed] 🎯 INTERVIEW MODE: Seeding 50 specific orders for the coding challenge..."
  
  # Produce messages from interview-orders.txt with key:value format
  /usr/bin/kafka-console-producer \
    --bootstrap-server "$BOOTSTRAP" \
    --topic "$TOPIC" \
    --property "parse.key=true" \
    --property "key.separator=:" \
    < /scripts/interview-orders.txt
  
  echo "[seed] ✅ Interview orders seeded successfully (50 messages across 3 partitions)."
else
  echo "[seed] Producing $COUNT JSON orders to topic '$TOPIC' at $BOOTSTRAP ..."
  
  # Generate COUNT JSON documents and pipe to kafka-console-producer
  for n in $(seq 1 "$COUNT"); do
    generate_order_json "$n"
    echo
  done | /usr/bin/kafka-console-producer --bootstrap-server "$BOOTSTRAP" --topic "$TOPIC" >/dev/null
  
  echo "[seed] Done."
fi

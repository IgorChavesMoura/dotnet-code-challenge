#!/usr/bin/env bash
set -euo pipefail

BOOTSTRAP="${BOOTSTRAP_SERVERS:-kafka:9092}"
PARTITIONS="${PARTITIONS:-3}"
RF="${RF:-1}"

echo "[init-topics] Waiting for Kafka at $BOOTSTRAP ..."
for i in {1..60}; do
  if /usr/bin/kafka-topics --bootstrap-server "$BOOTSTRAP" --list >/dev/null 2>&1; then
    break
  fi
  sleep 2
done

create_topic () {
  local topic="$1"
  local p="$2"
  local rf="$3"

  if /usr/bin/kafka-topics --bootstrap-server "$BOOTSTRAP" --list | grep -qx "$topic"; then
    echo "[init-topics] Topic exists: $topic"
  else
    echo "[init-topics] Creating topic: $topic (p=$p, rf=$rf)"
    /usr/bin/kafka-topics \
      --bootstrap-server "$BOOTSTRAP" \
      --create \
      --if-not-exists \
      --topic "$topic" \
      --partitions "$p" \
      --replication-factor "$rf"
  fi
}

# Challenge topics
create_topic "${TOPIC_ORDERS:-orders}"         "$PARTITIONS" "$RF"

echo "[init-topics] Done."

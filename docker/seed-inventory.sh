#!/bin/bash
set -e

echo "Waiting for MongoDB to be ready..."
sleep 5

echo "Seeding inventory data..."

mongosh "mongodb://admin:password@mongodb:27017/orders?authSource=admin" --eval '
db.inventory.deleteMany({});

const products = [
  { productId: "P-001", name: "Keyboard", availableQuantity: 5000, reservedQuantity: 0 },
  { productId: "P-002", name: "Mouse", availableQuantity: 8000, reservedQuantity: 0 },
  { productId: "P-003", name: "Monitor 24in", availableQuantity: 3000, reservedQuantity: 0 },
  { productId: "P-004", name: "Laptop Stand", availableQuantity: 4000, reservedQuantity: 0 },
  { productId: "P-005", name: "Dock", availableQuantity: 2500, reservedQuantity: 0 },
  { productId: "P-006", name: "Headset", availableQuantity: 6000, reservedQuantity: 0 },
  { productId: "P-007", name: "HDMI Cable", availableQuantity: 10000, reservedQuantity: 0 },
  { productId: "P-008", name: "USB-C Cable", availableQuantity: 12000, reservedQuantity: 0 },
  { productId: "P-009", name: "SSD 1TB", availableQuantity: 3500, reservedQuantity: 0 },
  { productId: "P-010", name: "Webcam", availableQuantity: 4500, reservedQuantity: 0 }
];

products.forEach(product => {
  product.lastUpdated = new Date();
});

const result = db.inventory.insertMany(products);
print("Inserted " + result.insertedIds.length + " inventory items");

db.inventory.find().forEach(printjson);
'

echo "Inventory seeding completed!"

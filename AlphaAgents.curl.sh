#!/bin/bash

# Geography Agent
curl -X POST http://localhost:5190/api/agent/geography \
  -H "Content-Type: application/json" \
  -d '{"question": "What is the capital of France?"}'

echo ""

# Math Agent
curl -X POST http://localhost:5190/api/agent/math \
  -H "Content-Type: application/json" \
  -d '{"question": "What is 15 * 23?"}'

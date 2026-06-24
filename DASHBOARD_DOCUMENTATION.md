# Executive Dashboard Documentation

## Overview
The Executive Dashboard provides real-time customer insights and analytics.

## Features
1. **KPI Cards** - Total, Active, Companies, Persons
2. **Pie Chart** - Customer type distribution
3. **Doughnut Chart** - Status distribution
4. **Bar Chart** - Analytics overview
5. **Business Type Distribution** - Top 5 business types
6. **Key Insights** - Most common type, active rate, ratios

## Data Source
- API: `http://196.191.244.156:7029/api/consignee/dynamic?gsltype=28`
- Total Records: ~5,979 customers

## Metrics Calculated
| Metric | Calculation |
|--------|-------------|
| Total Customers | Count all records |
| Active Customers | Count where isActive = true |
| Companies | Count where isPerson = false |
| Persons | Count where isPerson = true |
| Active Rate | Active / Total * 100 |
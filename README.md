# Product Discount Manager (Implementation)

## Overview

This solution consists of two main projects: an API and a Background Service. The API manages product information stored in an MSSQL database, while the service applies daily discounts to products based on XML files.

## Projects

**1. ProductAPI**

This ASP.NET API interacts with an Oracle database to manage product information. It supports operations to retrieve, add, and update product data, as well as to handle discounts through XML file processing.

**2. DiscountService**

A background service scheduled to run daily between 09:00 and 09:15. It checks for a corresponding XML discount file and applies discounts to products as specified.

## Features

GetProduct: Retrieves all product data from the database.

PostProduct: Adds or updates product data in the database.

PostDiscount: Processes XML files to save discount information.

SetDiscount: Applies discounts from the day's XML file to the database. If the price is set to 0 or the file is not found, the discount is not applied.

## Technologies

.NET Core 6-8

MSSQL Database

C#

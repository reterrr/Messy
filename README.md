# Messy App

Messy is an API-based chat application, developed with ASP.NET Core and PostgreSQL. This guide will help you set up and run the application locally using Docker.

## Prerequisites

Ensure you have the following installed on your machine:

- **Docker**: To build and run containers.
- **Docker Compose**: To manage multi-container applications.
- **PostgreSQL**: For the database.

## Quick Start Guide

### 1. Clone the repository

Start by cloning the repository:

```bash
git clone <repository_url>
cd messy
docker-compose up --build
psql -h postgres -U postgres -d messy -a -f Messy/Contexts/init.sql

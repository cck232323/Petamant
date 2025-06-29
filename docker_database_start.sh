# 1. Access the running PostgreSQL container
docker exec -it petamant-db-1 /bin/bash

# 2. Enter PostgreSQL shell as user "postgres"
# psql -U postgres

# \l                      -- List all databases
# \c petactivities        -- Connect to your database
# \d                      -- List all tables in the current database
# SELECT * FROM "Users";  -- Query data from the "Users" table
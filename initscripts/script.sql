ALTER USER postgres WITH PASSWORD '12345678';

-- Create the "messy" database
CREATE DATABASE messy;

-- Grant all privileges on the "messy" database to the "postgres" user
GRANT ALL PRIVILEGES ON DATABASE messy TO postgres;

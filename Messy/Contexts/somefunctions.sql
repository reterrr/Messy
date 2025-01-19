-- Function for user login
-- CREATE OR REPLACE FUNCTION login_user(username VARCHAR, password VARCHAR)
--     RETURNS TABLE(user_id BIGINT, user_name VARCHAR) AS $$
-- BEGIN
--     RETURN QUERY
--         SELECT id, name
--         FROM users
--         WHERE username = username AND password = password AND deletedat IS NULL;
-- END;
-- $$ LANGUAGE plpgsql;

-- Procedure for user registration
CREATE OR REPLACE PROCEDURE register_user(
    username VARCHAR,
    password VARCHAR,
    name VARCHAR
)
    LANGUAGE plpgsql AS
$$
BEGIN
    INSERT INTO users (name, username, password, createdat)
    VALUES (name, username, password, NOW());
    INSERT INTO userprofiles (userid, createdat)
    VALUES (currval('users_id_seq'), NOW());
END;
$$;

-- Procedure to delete a user
CREATE OR REPLACE PROCEDURE delete_user(user_id BIGINT)
    LANGUAGE plpgsql AS
$$
BEGIN
    UPDATE users SET deletedat = NOW() WHERE id = user_id;
    UPDATE userprofiles SET deletedat = NOW() WHERE userid = user_id;
END;
$$;

-- Procedure to edit user profile
CREATE OR REPLACE PROCEDURE edit_userprofile(
    user_id BIGINT,
    new_description VARCHAR DEFAULT NULL,
    new_images TEXT[] DEFAULT NULL
)
    LANGUAGE plpgsql AS
$$
BEGIN
    IF new_description IS NOT NULL THEN
        UPDATE userprofiles SET description = new_description, updatedat = NOW() WHERE userid = user_id;
    END IF;
    -- Add logic to handle new_images if file management is implemented
END;
$$;

-- Procedure to create a message
CREATE OR REPLACE PROCEDURE create_message(
    chat_user_id BIGINT,
    body TEXT,
    file_ids BIGINT[] DEFAULT NULL
)
    LANGUAGE plpgsql AS
$$
DECLARE
    message_id BIGINT;
BEGIN
    INSERT INTO messages (body, chatuserid, createdat)
    VALUES (body, chat_user_id, NOW())
    RETURNING id INTO message_id;

    IF file_ids IS NOT NULL THEN
        INSERT INTO messagefiles (messageid, fileid)
        SELECT message_id, UNNEST(file_ids);
    END IF;
END;
$$;

-- Procedure to delete a message
CREATE OR REPLACE PROCEDURE delete_message(message_id BIGINT)
    LANGUAGE plpgsql AS
$$
BEGIN
    UPDATE messages SET deletedat = NOW() WHERE id = message_id;
END;
$$;

-- Procedure to edit a message
CREATE OR REPLACE PROCEDURE edit_message(
    message_id BIGINT,
    new_body TEXT DEFAULT NULL,
    new_file_ids BIGINT[] DEFAULT NULL,
    new_parent_id BIGINT DEFAULT NULL
)
    LANGUAGE plpgsql AS
$$
BEGIN
    IF new_body IS NOT NULL THEN
        UPDATE messages SET body = new_body, updatedat = NOW() WHERE id = message_id;
    END IF;

    IF new_file_ids IS NOT NULL THEN
        DELETE FROM messagefiles WHERE messageid = message_id;
        INSERT INTO messagefiles (messageid, fileid)
        SELECT message_id, UNNEST(new_file_ids);
    END IF;

    IF new_parent_id IS NOT NULL THEN
        UPDATE messages SET parentid = new_parent_id, updatedat = NOW() WHERE id = message_id;
    END IF;
END;
$$;

-- Procedure to create a chat with users
CREATE OR REPLACE PROCEDURE create_chat(
    chat_title VARCHAR,
    chat_type INT,
    user_ids BIGINT[]
)
    LANGUAGE plpgsql AS
$$
DECLARE
    chat_id BIGINT;
BEGIN
    INSERT INTO chats (title, type, createdat)
    VALUES (chat_title, chat_type, NOW())
    RETURNING id INTO chat_id;

    INSERT INTO chatusers (userid, chatid, createdat)
    SELECT UNNEST(user_ids), chat_id, NOW();
END;
$$;

-- Function to filter users by name or username
CREATE OR REPLACE FUNCTION filter_users(search_name VARCHAR, search_username VARCHAR)
    RETURNS TABLE
            (
                user_id  BIGINT,
                name     VARCHAR,
                username VARCHAR
            )
AS
$$
BEGIN
    RETURN QUERY
        SELECT id, name, username
        FROM users
        WHERE (name ILIKE '%' || search_name || '%' OR username ILIKE '%' || search_username || '%')
          AND deletedat IS NULL;
END;
$$ LANGUAGE plpgsql;

-- Function to get chat details
CREATE OR REPLACE FUNCTION get_chat(chat_id BIGINT)
    RETURNS TABLE
            (
                chat_title VARCHAR,
                chat_type  INT,
                users      JSON
            )
AS
$$
BEGIN
    RETURN QUERY
        SELECT title, type, JSON_AGG(json_build_object('user_id', cu.userid, 'joined_at', cu.createdat))
        FROM chats c
                 JOIN chatusers cu ON c.id = cu.chatid
        WHERE c.id = chat_id
        GROUP BY c.id;
END;
$$ LANGUAGE plpgsql;

-- Function to filter chats by title
CREATE OR REPLACE FUNCTION filter_chats(search_title VARCHAR)
    RETURNS TABLE
            (
                chat_id BIGINT,
                title   VARCHAR,
                type    INT
            )
AS
$$
BEGIN
    RETURN QUERY
        SELECT id, title, type
        FROM chats
        WHERE title ILIKE '%' || search_title || '%'
          AND deletedat IS NULL;
END;
$$ LANGUAGE plpgsql;

-- Procedure to add users to a group chat
CREATE OR REPLACE PROCEDURE add_to_group(chat_id BIGINT, user_ids BIGINT[])
    LANGUAGE plpgsql AS
$$
BEGIN
    INSERT INTO chatusers (userid, chatid, createdat)
    SELECT UNNEST(user_ids), chat_id, NOW()
    ON CONFLICT (userid, chatid) DO NOTHING;
END;
$$;

CREATE OR REPLACE FUNCTION user_exists(matching TEXT)
    RETURNS BOOLEAN AS
$$
BEGIN
    RETURN EXISTS (SELECT 1
                   FROM users
                   WHERE username = matching
                     AND deletedat IS NULL);
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION get_user(user_id BIGINT)
    RETURNS users AS $$
BEGIN
    RETURN (SELECT * FROM users WHERE users.id = user_id);
END;
$$ LANGUAGE plpgsql;


select user_exists('hell');



create or replace function delete_user(userId bigint)
    RETURNS BOOLEAN
    language plpgsql as
$$
DECLARE
    deleted BOOLEAN;
BEGIN
    DELETE FROM users WHERE id = userId RETURNING TRUE INTO deleted;

    RETURN COALESCE(deleted, FALSE);
end;
$$
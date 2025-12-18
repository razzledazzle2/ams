
CREATE FUNCTION get_user_by_username(p_username TEXT)
RETURNS SETOF users
AS $$
BEGIN
    RETURN QUERY
    SELECT *
    FROM users
    WHERE username = p_username;
END;
$$ LANGUAGE plpgsql;

CREATE FUNCTION get_user_by_id(p_id UUID)
RETURNS SETOF users
AS $$
BEGIN
    RETURN QUERY
    SELECT *
    FROM users
    WHERE id = p_id;
END;
$$ LANGUAGE plpgsql;

CREATE FUNCTION create_user(
    p_username TEXT,
    p_password_hash TEXT
)
RETURNS UUID
AS $$
DECLARE
    new_id UUID;
BEGIN
    INSERT INTO users (
        username,
        password_hash,
        created_at
    )
    VALUES (
        p_username,
        p_password_hash,
        NOW()
    )
    RETURNING id INTO new_id;

    RETURN new_id;
END;
$$ LANGUAGE plpgsql;


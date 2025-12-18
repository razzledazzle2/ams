DROP FUNCTION IF EXISTS create_refresh_token(
    UUID, UUID, TIMESTAMPTZ, TIMESTAMPTZ
);

CREATE FUNCTION create_refresh_token(
    p_id UUID,
    p_user_id UUID,
    p_expires_at TIMESTAMPTZ,
    p_created_at TIMESTAMPTZ
)
RETURNS VOID
AS $$
BEGIN
    INSERT INTO refresh_tokens (id, user_id, expires_at, created_at)
    VALUES (p_id, p_user_id, p_expires_at, p_created_at);
END;
$$ LANGUAGE plpgsql;
DROP FUNCTION IF EXISTS get_refresh_token_by_id(UUID);

CREATE FUNCTION get_refresh_token_by_id(p_id UUID)
RETURNS SETOF refresh_tokens
AS $$
BEGIN
    RETURN QUERY
    SELECT *
    FROM refresh_tokens
    WHERE id = p_id;
END;
$$ LANGUAGE plpgsql;
DROP FUNCTION IF EXISTS revoke_refresh_token(UUID);

CREATE FUNCTION revoke_refresh_token(p_id UUID)
RETURNS VOID
AS $$
BEGIN
    UPDATE refresh_tokens
    SET revoked_at = NOW()
    WHERE id = p_id;
END;
$$ LANGUAGE plpgsql;
DROP FUNCTION IF EXISTS revoke_all_refresh_tokens_for_user(UUID);

CREATE FUNCTION revoke_all_refresh_tokens_for_user(p_user_id UUID)
RETURNS VOID
AS $$
BEGIN
    UPDATE refresh_tokens
    SET revoked_at = NOW()
    WHERE user_id = p_user_id
      AND revoked_at IS NULL;
END;
$$ LANGUAGE plpgsql;

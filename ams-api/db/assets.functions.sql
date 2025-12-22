
CREATE FUNCTION get_all_assets()
RETURNS SETOF assets
AS $$
BEGIN
    RETURN QUERY
    SELECT *
    FROM assets;
END;
$$ LANGUAGE plpgsql;


CREATE FUNCTION get_asset_by_id(p_id UUID)
RETURNS SETOF assets
AS $$
BEGIN
    RETURN QUERY
    SELECT *
    FROM assets
    WHERE id = p_id;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION create_asset(
    p_name TEXT,
    p_category TEXT,
    p_status TEXT,
    p_condition TEXT,
    p_purchase_date TIMESTAMP,
    p_vendor TEXT,
    p_created_by UUID
)
RETURNS UUID
LANGUAGE plpgsql
AS $$
DECLARE
    new_id UUID;
BEGIN
    INSERT INTO assets (
        name,
        category,
        status,
        condition,
        purchase_date,
        vendor,
        created_by,
        created_at,
        updated_at
    )
    VALUES (
        p_name,
        p_category,
        p_status,
        p_condition,
        p_purchase_date,
        p_vendor,
        p_created_by,
        NOW(),
        NOW()
    )
    RETURNING id INTO new_id;

    RETURN new_id;
END;
$$;

CREATE OR REPLACE FUNCTION update_asset(
    p_id UUID,
    p_name TEXT,
    p_category TEXT,
    p_status TEXT,
    p_condition TEXT,
    p_purchase_date TIMESTAMP,
    p_vendor TEXT
)
RETURNS BOOLEAN
AS $$
BEGIN
    UPDATE assets
    SET
        name = p_name,
        category = p_category,
        status = p_status,
        condition = p_condition,
        purchase_date = p_purchase_date,
        vendor = p_vendor,
        updated_at = NOW()
    WHERE id = p_id;

    RETURN FOUND;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION delete_asset(p_id UUID)
RETURNS BOOLEAN
AS $$
BEGIN
    DELETE FROM assets WHERE id = p_id;
    RETURN FOUND;
END;
$$ LANGUAGE plpgsql;

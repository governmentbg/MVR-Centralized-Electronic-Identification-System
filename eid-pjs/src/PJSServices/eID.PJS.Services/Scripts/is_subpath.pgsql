CREATE OR REPLACE FUNCTION is_subpath(base_path text, path text)
RETURNS boolean AS $$
DECLARE
    normalized_base_path text;
    normalized_path text;
BEGIN
    -- Normalize paths by replacing backslashes with forward slashes (for Windows paths)
    normalized_base_path := replace(rtrim(base_path, '/\') || '/', '\', '/');
    normalized_path := replace(rtrim(path, '/\') || '/', '\', '/');

    -- Check if the paths are identical
    IF normalized_base_path ILIKE normalized_path THEN
        RETURN true;
    END IF;

    -- Check if the path starts with the base path
    -- Using ILIKE for case-insensitive comparison, accommodating Windows paths
    RETURN normalized_path ILIKE (normalized_base_path || '%');
END;
$$ LANGUAGE plpgsql IMMUTABLE STRICT;

-- Example Usage:
-- SELECT is_subpath('/parent/folder', '/parent/folder/subfolder/file.txt');
-- SELECT is_subpath('C:\ParentFolder', 'C:\ParentFolder\SubFolder\file.txt');
-- SELECT is_subpath('/parent/folder', '/parent/folder');
-- All should return true


-----------------------------------------------------------

-- FUNCTION: public.is_subpath(text, text)

-- DROP FUNCTION IF EXISTS public.is_subpath(text, text);

CREATE OR REPLACE FUNCTION public.is_subpath(base_path text, path text)
    RETURNS boolean
    LANGUAGE 'plpgsql'
    COST 100
    IMMUTABLE STRICT PARALLEL UNSAFE
AS $BODY$
DECLARE
    normalized_base_path text;
    normalized_path text;
BEGIN
    -- Normalize paths by replacing backslashes with forward slashes (for Windows paths)
    normalized_base_path := replace(rtrim(base_path, '/\') || '/', '\', '/');
    normalized_path := replace(rtrim(path, '/\') || '/', '\', '/');

    -- Check if the paths are identical
    IF normalized_base_path ILIKE normalized_path THEN
        RETURN true;
    END IF;

    -- Check if the path starts with the base path
    -- Using ILIKE for case-insensitive comparison, accommodating Windows paths
    RETURN normalized_path ILIKE (normalized_base_path || '%');
END;
$BODY$;

ALTER FUNCTION public.is_subpath(text, text)
    OWNER TO postgres;
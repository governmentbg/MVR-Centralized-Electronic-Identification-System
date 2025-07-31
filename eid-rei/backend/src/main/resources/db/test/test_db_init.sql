CREATE SCHEMA IF NOT EXISTS rei;
SET SCHEMA rei;

CREATE ALIAS IF NOT EXISTS PGP_SYM_DECRYPT AS $$
@CODE
String PGP_SYM_DECRYPT(byte[] inputBytes, String password) {
    return new String(inputBytes);
}
$$;

CREATE ALIAS IF NOT EXISTS PGP_SYM_ENCRYPT AS $$
@CODE
byte[] PGP_SYM_ENCRYPT(String input, String password) {
    return input.getBytes();
}
$$;
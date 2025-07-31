package com.digitall.eid.data.models.keystore

data class KeystoreEncryptionSecret(
    val alias: String,
    val data: String,
)

data class KeystoreDecryptionSecret(
    val alias: String,
    val data: String,
    val iv: String,
)
package com.digitall.eid.data.utils

import android.security.keystore.KeyGenParameterSpec
import android.security.keystore.KeyProperties
import android.util.Base64
import com.digitall.eid.data.BuildConfig.ANDROID_KEYSTORE
import com.digitall.eid.data.models.keystore.KeystoreDecryptionSecret
import com.digitall.eid.data.models.keystore.KeystoreEncryptionSecret
import java.security.Key
import java.security.KeyStore
import javax.crypto.Cipher
import javax.crypto.KeyGenerator
import javax.crypto.spec.GCMParameterSpec

class KeystoreHelper {

    companion object {
        private const val TRANSFORMATION = "AES/GCM/NoPadding"
        private const val EMPTY = ""
        private const val NEW_LINE = "\n"
        private const val KEY_SIZE = 256
        private const val GCM_KEY_SIZE = 128
    }

    private lateinit var keyStore: KeyStore

    init {
        initializeKeystore()
    }

    fun encrypt(encryptionSecret: KeystoreEncryptionSecret): KeystoreDecryptionSecret {
        val secKey = getKey(encryptionSecret.alias)
        val cipher = Cipher.getInstance(TRANSFORMATION)
        cipher.init(Cipher.ENCRYPT_MODE, secKey)

        val encryptedData = cipher.doFinal(encryptionSecret.data.toByteArray())
        val encryptedDataBase64 = Base64.encodeToString(encryptedData, Base64.NO_PADDING)
        val ivBase64 = Base64.encodeToString(cipher.iv, Base64.NO_PADDING)

        return KeystoreDecryptionSecret(
            data = encryptedDataBase64.replace(NEW_LINE, EMPTY),
            iv = ivBase64.replace(NEW_LINE, EMPTY),
            alias = encryptionSecret.alias
        )
    }

    fun decrypt(decryptionSecret: KeystoreDecryptionSecret): KeystoreEncryptionSecret {
        val iv = Base64.decode(decryptionSecret.iv, Base64.NO_PADDING)
        val data = Base64.decode(decryptionSecret.data, Base64.NO_PADDING)

        val secretKeyEntry =
            keyStore.getEntry(decryptionSecret.alias, null) as KeyStore.SecretKeyEntry
        val cipher = Cipher.getInstance(TRANSFORMATION)
        val gmcParametersSpec = GCMParameterSpec(GCM_KEY_SIZE, iv)
        cipher.init(Cipher.DECRYPT_MODE, secretKeyEntry.secretKey, gmcParametersSpec)

        val decryptedData = String(cipher.doFinal(data))

        return KeystoreEncryptionSecret(
            alias = decryptionSecret.alias,
            data = decryptedData,
        )
    }

    private fun initializeKeystore() {
        keyStore = KeyStore.getInstance(ANDROID_KEYSTORE)
        keyStore.load(null)
    }

    private fun getKey(alias: String): Key {
        if (keyStore.containsAlias(alias)) {
            return keyStore.getKey(alias, null)
        }

        val generator = KeyGenerator.getInstance(KeyProperties.KEY_ALGORITHM_AES, ANDROID_KEYSTORE)
        val keyGenParameterSpec = KeyGenParameterSpec.Builder(
            alias,
            KeyProperties.PURPOSE_ENCRYPT or KeyProperties.PURPOSE_DECRYPT
        ).setBlockModes(KeyProperties.BLOCK_MODE_GCM)
            .setKeySize(KEY_SIZE)
            .setEncryptionPaddings(KeyProperties.ENCRYPTION_PADDING_NONE)
            .build()

        generator.init(keyGenParameterSpec)

        return generator.generateKey()
    }
}
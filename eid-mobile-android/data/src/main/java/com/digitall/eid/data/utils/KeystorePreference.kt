package com.digitall.eid.data.utils

import android.app.Application
import android.content.Context
import android.content.SharedPreferences
import androidx.core.content.edit
import com.digitall.eid.data.models.keystore.KeystoreDecryptionSecret
import com.digitall.eid.data.models.keystore.KeystoreEncryptionSecret
import org.json.JSONObject
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

class KeystorePreference(private val application: Application) : KoinComponent {

    companion object {
        private const val SP_NAME = "__eid_encrypted_data"
    }

    private val keystoreHelper: KeystoreHelper by inject()

    private val sharedPreferences: SharedPreferences by lazy {
        application.getSharedPreferences(SP_NAME, Context.MODE_PRIVATE)
    }

    fun get(key: String): String? {
        val json = sharedPreferences.getString(key, null) ?: return null

        val `object` = JSONObject(json)

        val decryptionSecret = KeystoreDecryptionSecret(
            alias = `object`.getString("alias"),
            iv = `object`.getString("iv"),
            data = `object`.getString("data"),
        )

        return keystoreHelper.decrypt(decryptionSecret).data
    }

    @Throws
    fun save(key: String, value: String): KeystoreDecryptionSecret {
        if (sharedPreferences.contains(key)) {
            remove(key = key)
        }

        val input = KeystoreEncryptionSecret(
            alias = key,
            data = value
        )
        val encrypted = keystoreHelper.encrypt(encryptionSecret = input)

        val json = JSONObject().apply {
            put("alias", encrypted.alias)
            put("data", encrypted.data)
            put("iv", encrypted.iv)
        }
        sharedPreferences.edit { putString(key, json.toString()) }

        return encrypted
    }

    fun remove(key: String) {
        sharedPreferences.edit { remove(key) }
    }
}
/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.utils

import com.digitall.eid.domain.models.csr.algorithm.CsrAlgorithm
import com.digitall.eid.domain.models.csr.principal.CsrPrincipalModel
import java.io.InputStream
import java.security.KeyPair
import java.security.PrivateKey
import java.security.cert.Certificate
import javax.crypto.Cipher
import javax.net.ssl.SSLContext
import javax.net.ssl.X509TrustManager

interface CryptographyHelper {

    fun encrypt(text: String, cipher: Cipher? = null): String

    fun decrypt(text: String, cipher: Cipher? = null): String

    fun getOrCreateSharedPreferencesKey(): String

    fun createDatabaseKey(): String

    fun getBiometricCipherForEncryption(): Cipher?

    fun getBiometricCipherForDecryption(initializationVector: ByteArray): Cipher?

    fun getInitializationVectorFromString(text: String): ByteArray

    /**
     * Generates a pair of public and private keys with unique [alias].
     * This keys could be used for the exchange and then you can decrypt the
     * content with private key in [decryptWithPrivateKey].
     */
    fun generatePublicPrivateKeyPair(alias: String): KeyPair

    fun decryptWithPrivateKey(privateKey: PrivateKey, text: String): String

    /**
     * Encrypt the [text] with [publicKey] from server and returns
     * the Base64 string to send to server. Usually, uses the same algorithms as
     * [decryptWithPrivateKey] and [generatePublicPrivateKeyPair].
     */
    fun encryptWithPublicServerKey(publicKey: String, text: String): String

    /**
     * Generate SSL context form incoming certificate. Certificate should be in .p12 format.
     * Note that the input stream will be closed after ssl context generation.
     */
    fun generateSslContextAndTrustManager(
        certificate: InputStream,
        password: String
    ): Pair<SSLContext?, X509TrustManager?>

    fun generateCSR(
        keyAlias: String,
        algorithm: CsrAlgorithm,
        principal: CsrPrincipalModel,
    ): Pair<PrivateKey?, String?>


    fun saveCertificateToKeyStore(
        alias: String,
        certificate: String,
    ): Boolean

    fun saveCertificateWithChainToKeyStore(
        alias: String,
        certificate: String,
        certificateChain: List<String>,
    ): Boolean

    fun saveCertificateWithChainToKeyStore(
        alias: String,
        certificate: String,
        certificateChain: String,
    ): Boolean

    fun deleteCertificateFromKeyStore(
        alias: String,
    )

    fun deleteCertificateWithChainFromKeyStore(
        alias: String,
    )

    fun deleteCertificateWithChainFromKeyStore(
        alias: String,
        chainCount: Int,
    )

    fun getPrivateKey(alias: String): PrivateKey?

    fun deletePrivateKey(alias: String)

    fun getCertificate(alias: String): Certificate?

    fun getCertificateChain(alias: String, chainCount: Int): Array<Certificate>

    fun hasAlias(alias: String): Boolean
}
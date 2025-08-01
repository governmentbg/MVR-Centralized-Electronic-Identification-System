/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.utils

import android.content.Context
import android.content.pm.PackageManager
import android.os.Build
import android.security.keystore.KeyGenParameterSpec
import android.security.keystore.KeyProperties
import android.util.Base64
import com.digitall.eid.data.BuildConfig.ANDROID_KEYSTORE
import com.digitall.eid.data.BuildConfig.BIOMETRIC_KEY_ALIAS
import com.digitall.eid.data.BuildConfig.MASTER_PREFERENCES_KEY_ALIAS
import com.digitall.eid.data.BuildConfig.MASTER_SYMMETRIC_KEY_ALIAS
import com.digitall.eid.domain.extensions.toBase64
import com.digitall.eid.domain.models.csr.algorithm.CsrAlgorithm
import com.digitall.eid.domain.models.csr.principal.CsrPrincipalModel
import com.digitall.eid.domain.utils.CryptographyHelper
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import okhttp3.internal.platform.Platform
import org.bouncycastle.asn1.ASN1Encodable
import org.bouncycastle.asn1.DERSequence
import org.bouncycastle.asn1.x509.BasicConstraints
import org.bouncycastle.asn1.x509.ExtendedKeyUsage
import org.bouncycastle.asn1.x509.Extension
import org.bouncycastle.asn1.x509.GeneralName
import org.bouncycastle.asn1.x509.KeyPurposeId
import org.bouncycastle.asn1.x509.KeyUsage
import org.bouncycastle.cert.X509v3CertificateBuilder
import org.bouncycastle.cert.jcajce.JcaX509CertificateConverter
import org.bouncycastle.cert.jcajce.JcaX509v3CertificateBuilder
import org.bouncycastle.jce.provider.BouncyCastleProvider
import org.bouncycastle.operator.jcajce.JcaContentSignerBuilder
import org.bouncycastle.pkcs.PKCS10CertificationRequest
import org.bouncycastle.pkcs.jcajce.JcaPKCS10CertificationRequestBuilder
import org.bouncycastle.util.io.pem.PemObject
import org.bouncycastle.util.io.pem.PemWriter
import java.io.InputStream
import java.io.StringWriter
import java.math.BigInteger
import java.security.KeyFactory
import java.security.KeyPair
import java.security.KeyPairGenerator
import java.security.KeyStore
import java.security.KeyStore.PrivateKeyEntry
import java.security.PrivateKey
import java.security.cert.Certificate
import java.security.cert.CertificateFactory
import java.security.cert.X509Certificate
import java.security.spec.ECGenParameterSpec
import java.security.spec.RSAKeyGenParameterSpec
import java.security.spec.X509EncodedKeySpec
import java.util.Date
import java.util.UUID
import javax.crypto.Cipher
import javax.crypto.KeyGenerator
import javax.crypto.SecretKey
import javax.crypto.spec.GCMParameterSpec
import javax.net.ssl.KeyManagerFactory
import javax.net.ssl.SSLContext
import javax.net.ssl.X509TrustManager
import javax.security.auth.x500.X500Principal


class CryptographyHelperImpl(
    private val context: Context
) : CryptographyHelper {

    companion object {
        private const val TAG = "CryptographyRepositoryImplTag"

        // Symmetric encryption
        private const val ENCRYPTION_BLOCK_MODE = KeyProperties.BLOCK_MODE_GCM
        private const val ENCRYPTION_PADDING = KeyProperties.ENCRYPTION_PADDING_NONE
        private const val ENCRYPTION_ALGORITHM = KeyProperties.KEY_ALGORITHM_AES
        private const val SYMMETRIC_KEY_SIZE = 256
        private const val GCM_TAG_LENGTH_BITS = 128

        // Asymmetric encryption
        private const val AE_ENCRYPTION_BLOCK_MODE = KeyProperties.BLOCK_MODE_ECB
        private const val AE_ENCRYPTION_PADDING = KeyProperties.ENCRYPTION_PADDING_RSA_PKCS1
        private const val AE_ENCRYPTION_ALGORITHM = KeyProperties.KEY_ALGORITHM_RSA
        private const val ASYMMETRIC_KEY_SIZE = 2048
        private const val IV_SEPARATOR = "###"

        private const val CERTIFICATE_FACTORY_TYPE = "X.509"
        private const val CSR_ALGORITHM = "secp256r1"
        private const val KEY_PAIR_GENERATOR_PROVIDER = "AndroidKeyStore"
    }

    private lateinit var keyStore: KeyStore

    init {
        initializeKeystore()
    }

    private fun initializeKeystore() {
        keyStore = KeyStore.getInstance(ANDROID_KEYSTORE)
        keyStore.load(null)
    }

    override fun encrypt(text: String, cipher: Cipher?): String {
        val result = StringBuilder()
        val encryptionCipher = cipher ?: getInitializedCipherForEncryption()
        // Add IV
        val ivString = Base64.encodeToString(encryptionCipher.iv, Base64.NO_WRAP)
        result.append(ivString + IV_SEPARATOR)
        // Encrypt
        val ciphertext = encryptionCipher.doFinal(text.toByteArray())
        val encodedString = Base64.encodeToString(ciphertext, Base64.NO_WRAP)
        result.append(encodedString)
        return result.toString()
    }

    override fun decrypt(text: String, cipher: Cipher?): String {
        // Get IV from text
        val split = text.split(IV_SEPARATOR)
        val ivString = split[0]
        val encodedString = split[1]
        val initializationVector = Base64.decode(ivString, Base64.NO_WRAP)
        val decryptionCipher = cipher ?: getInitializedCipherForDecryption(initializationVector)
        // Decrypt
        val decodedString = Base64.decode(encodedString, Base64.NO_WRAP)
        val result = decryptionCipher.doFinal(decodedString)
        return String(result)
    }

    override fun getOrCreateSharedPreferencesKey(): String {
        // Create a shared preferences key
        return MASTER_PREFERENCES_KEY_ALIAS
    }

    override fun createDatabaseKey(): String {
        return UUID.randomUUID().toString()
    }

    override fun getBiometricCipherForEncryption(): Cipher? {
        return try {
            val cipher = getSymmetricCipher()
            val secretKey = getOrCreateBiometricSecretKey()
            cipher.init(Cipher.ENCRYPT_MODE, secretKey)
            cipher
        } catch (e: Exception) {
            logError("getBiometricCipherForEncryption Exception: ${e.message}", e, TAG)
            null
        }
    }

    override fun getBiometricCipherForDecryption(initializationVector: ByteArray): Cipher? {
        return try {
            val cipher = getSymmetricCipher()
            val secretKey = getOrCreateBiometricSecretKey()
            cipher.init(Cipher.DECRYPT_MODE, secretKey, GCMParameterSpec(GCM_TAG_LENGTH_BITS, initializationVector))
            cipher
        } catch (e: Exception) {
            logError("getBiometricCipherForDecryption Exception: ${e.message}", e, TAG)
            null
        }
    }

    override fun getInitializationVectorFromString(text: String): ByteArray {
        val split = text.split(IV_SEPARATOR)
        val ivString = split[0]
        return Base64.decode(ivString, Base64.NO_WRAP)
    }

    override fun generatePublicPrivateKeyPair(alias: String): KeyPair {
        val generator = KeyPairGenerator.getInstance(AE_ENCRYPTION_ALGORITHM, ANDROID_KEYSTORE)
        val purpose = KeyProperties.PURPOSE_ENCRYPT or KeyProperties.PURPOSE_DECRYPT
        val builder = KeyGenParameterSpec.Builder(alias, purpose)
            .setBlockModes(AE_ENCRYPTION_BLOCK_MODE)
            .setEncryptionPaddings(AE_ENCRYPTION_PADDING)
            .setKeySize(ASYMMETRIC_KEY_SIZE)
        generator.initialize(builder.build())
        return generator.generateKeyPair()
    }

    override fun decryptWithPrivateKey(privateKey: PrivateKey, text: String): String {
        val cipher = Cipher.getInstance(privateKey.algorithm)
        cipher.init(Cipher.DECRYPT_MODE, privateKey)
        // Decrypt
        val decodedString = Base64.decode(text, Base64.NO_WRAP)
        val result = cipher.doFinal(decodedString)
        return String(result)
    }

    override fun encryptWithPublicServerKey(publicKey: String, text: String): String {
        val normPublicKey = normalizePublicKeyWhenNeeded(publicKey)
        val publicBytes = Base64.decode(normPublicKey, Base64.DEFAULT)
        val keySpec = X509EncodedKeySpec(publicBytes)
        val keyFactory = KeyFactory.getInstance(AE_ENCRYPTION_ALGORITHM)
        val finalPublicKey = keyFactory.generatePublic(keySpec)
        val transformation =
            "$AE_ENCRYPTION_ALGORITHM/$AE_ENCRYPTION_BLOCK_MODE/$AE_ENCRYPTION_PADDING"
        val cipher = Cipher.getInstance(transformation)
        cipher.init(Cipher.ENCRYPT_MODE, finalPublicKey)
        // Encrypt
        val ciphertext = cipher.doFinal(text.toByteArray())
        return Base64.encodeToString(ciphertext, Base64.NO_WRAP)
    }

    private fun normalizePublicKeyWhenNeeded(publicKey: String): String {
        return publicKey.replace("\\r".toRegex(), "")
            .replace("\\n".toRegex(), "")
            .replace(System.lineSeparator().toRegex(), "")
            .replace("-----BEGIN PUBLIC KEY-----", "")
            .replace("-----END PUBLIC KEY-----", "")
    }

    override fun generateSslContextAndTrustManager(
        certificate: InputStream,
        password: String
    ): Pair<SSLContext?, X509TrustManager?> {
        val keyStore = KeyStore.getInstance("PKCS12")
        keyStore.load(certificate, password.toCharArray())
        val kmf: KeyManagerFactory = KeyManagerFactory.getInstance("X509")
        kmf.init(keyStore, UUID.randomUUID().toString().toCharArray())
        val trustManager = Platform.get().platformTrustManager()
        val sslContext = Platform.get().newSSLContext()
        sslContext.init(kmf.keyManagers, arrayOf(trustManager), null)
        certificate.close()

        return sslContext to trustManager
    }

    override fun generateCSR(
        keyAlias: String,
        algorithm: CsrAlgorithm,
        principal: CsrPrincipalModel
    ): Pair<PrivateKey?, String?> {
        logDebug("generateCSR", TAG)
        return try {
            val keyPairGenerator = when (algorithm) {
                CsrAlgorithm.RSA_1024,
                CsrAlgorithm.RSA_2048,
                CsrAlgorithm.RSA_3072 -> KeyPairGenerator.getInstance(
                    KeyProperties.KEY_ALGORITHM_RSA,
                    BouncyCastleProvider.PROVIDER_NAME
                )

                else -> KeyPairGenerator.getInstance(
                    KeyProperties.KEY_ALGORITHM_EC,
                    ANDROID_KEYSTORE
                )
            }

            val keyGenParameterSpec = KeyGenParameterSpec.Builder(
                keyAlias,
                KeyProperties.PURPOSE_SIGN or KeyProperties.PURPOSE_VERIFY
            ).apply {
                when (algorithm) {
                    CsrAlgorithm.RSA_1024,
                    CsrAlgorithm.RSA_2048,
                    CsrAlgorithm.RSA_3072 -> {
                        setAlgorithmParameterSpec(
                            RSAKeyGenParameterSpec(
                                algorithm.keySize,
                                RSAKeyGenParameterSpec.F4
                            )
                        )
                        setSignaturePaddings(
                            KeyProperties.SIGNATURE_PADDING_RSA_PKCS1,
                            KeyProperties.SIGNATURE_PADDING_RSA_PSS
                        )
                    }

                    else -> {

                        setAlgorithmParameterSpec(ECGenParameterSpec(algorithm.type))
                    }
                }

                setKeySize(algorithm.keySize)
                setDigests(KeyProperties.DIGEST_SHA256)
                setUserAuthenticationRequired(false)
            }.build()

            when (algorithm) {
                CsrAlgorithm.RSA_1024,
                CsrAlgorithm.RSA_2048,
                CsrAlgorithm.RSA_3072 -> keyPairGenerator.initialize(keyGenParameterSpec.algorithmParameterSpec)

                else -> keyPairGenerator.initialize(keyGenParameterSpec)
            }

            val keyPair = keyPairGenerator.generateKeyPair()
            logDebug("Public key: ${keyPair.public.toBase64()}", TAG)
            val x500Principal =
                X500Principal("CN=${principal.name}, GIVENNAME=${principal.givenName}, SURNAME=${principal.surname}, C=${principal.country}, SERIALNUMBER=${principal.serialNumber}")
            val csrBuilder = JcaPKCS10CertificationRequestBuilder(x500Principal, keyPair.public)
            val signerBuilder = when (algorithm) {
                CsrAlgorithm.RSA_1024,
                CsrAlgorithm.RSA_2048,
                CsrAlgorithm.RSA_3072 -> JcaContentSignerBuilder("SHA256withRSAandMGF1").setProvider(
                    BouncyCastleProvider.PROVIDER_NAME
                )

                else -> JcaContentSignerBuilder("SHA256WithECDSA")
            }

            val csr =
                csrBuilder.build(signerBuilder.build(keyPair.private))
            val result = csr.toPEM()
            logDebug("result: $result", TAG)

            when (algorithm) {
                CsrAlgorithm.RSA_1024,
                CsrAlgorithm.RSA_2048,
                CsrAlgorithm.RSA_3072 ->
                    keyStore.setKeyEntry(
                        keyAlias,
                        keyPair.private,
                        null,
                        arrayOf(
                            generateSelfSignedCertificateSecret(
                                host = keyAlias,
                                keyPair = keyPair
                            )
                        )
                    )

                else -> {}
            }

            keyPair.private to result
        } catch (exception: Exception) {
            logError("generateCSR Exception: ${exception.message}", exception, TAG)
            null to null
        }
    }

    override fun saveCertificateToKeyStore(alias: String, certificate: String): Boolean {
        logDebug("saveCertificateToKeyStore", TAG)
        return try {
            val certificateFactory = CertificateFactory.getInstance(CERTIFICATE_FACTORY_TYPE)
            val mainCertificate = decodeCertificate(certificate, certificateFactory)
            keyStore.setCertificateEntry(alias, mainCertificate)
            true
        } catch (e: Exception) {
            logError("saveCertificateToKeyStore Exception: ${e.message}", e, TAG)
            false
        }
    }

    override fun saveCertificateWithChainToKeyStore(
        alias: String,
        certificate: String,
        certificateChain: List<String>,
    ): Boolean {
        logDebug("saveCertificateWithChainToKeyStore", TAG)
        return try {
            val certificateFactory = CertificateFactory.getInstance(CERTIFICATE_FACTORY_TYPE)
            val mainCertificate = decodeCertificate(certificate, certificateFactory)
            keyStore.setCertificateEntry(alias, mainCertificate)
            certificateChain.forEachIndexed { index, cert ->
                val chainCertificate = decodeCertificate(cert, certificateFactory)
                keyStore.setCertificateEntry("$alias-chain-$index", chainCertificate)
            }
            true
        } catch (e: Exception) {
            logError("saveCertificateWithChainToKeyStore Exception: ${e.message}", e, TAG)
            false
        }
    }

    override fun saveCertificateWithChainToKeyStore(
        alias: String,
        certificate: String,
        certificateChain: String
    ): Boolean {
        logDebug("saveCertificateWithChainToKeyStore", TAG)
        return try {
            val certificateFactory = CertificateFactory.getInstance(CERTIFICATE_FACTORY_TYPE)
            val mainCertificate = decodeCertificate(certificate, certificateFactory)
            keyStore.setCertificateEntry(alias, mainCertificate)
            val chainCertificate = decodeCertificate(certificateChain, certificateFactory)
            keyStore.setCertificateEntry("$alias-chain", chainCertificate)
            true
        } catch (e: Exception) {
            logError("saveCertificateWithChainToKeyStore Exception: ${e.message}", e, TAG)
            false
        }
    }

    override fun deleteCertificateFromKeyStore(
        alias: String,
    ) {
        logDebug("deleteCertificateFromKeyStore", TAG)
        try {
            if (keyStore.containsAlias(alias)) {
                keyStore.deleteEntry(alias)
                logDebug("Main certificate with alias $alias deleted", TAG)
            } else {
                logDebug("No main certificate found with alias $alias", TAG)
            }
        } catch (e: Exception) {
            logError("deleteCertificateFromKeyStore Exception: ${e.message}", e, TAG)
        }
    }

    override fun deleteCertificateWithChainFromKeyStore(
        alias: String,
    ) {
        logDebug("deleteCertificateWithChainFromKeyStore", TAG)
        try {
            if (keyStore.containsAlias(alias)) {
                keyStore.deleteEntry(alias)
                logDebug("Main certificate with alias $alias deleted", TAG)
            } else {
                logDebug("No main certificate found with alias $alias", TAG)
            }
        } catch (e: Exception) {
            logError("deleteCertificateWithChainFromKeyStore Exception: ${e.message}", e, TAG)
        }
        val chainAlias = "$alias-chain"
        try {
            if (keyStore.containsAlias(chainAlias)) {
                keyStore.deleteEntry(chainAlias)
                logDebug("Chain certificate with alias $chainAlias deleted", TAG)
            } else {
                logDebug("No chain certificate found with alias $chainAlias", TAG)
            }
        } catch (e: Exception) {
            logError("deleteCertificateWithChainFromKeyStore Exception: ${e.message}", e, TAG)
        }
    }

    override fun deleteCertificateWithChainFromKeyStore(
        alias: String,
        chainCount: Int,
    ) {
        logDebug("deleteCertificateWithChainFromKeyStore", TAG)
        try {
            if (keyStore.containsAlias(alias)) {
                keyStore.deleteEntry(alias)
                logDebug("Main certificate with alias $alias deleted", TAG)
            } else {
                logDebug("No main certificate found with alias $alias", TAG)
            }
        } catch (e: Exception) {
            logError("deleteCertificateWithChainFromKeyStore Exception: ${e.message}", e, TAG)
        }
        for (i in 0 until chainCount) {
            val chainAlias = "$alias-chain-$i"
            try {
                if (keyStore.containsAlias(chainAlias)) {
                    keyStore.deleteEntry(chainAlias)
                    logDebug("Chain certificate with alias $chainAlias deleted", TAG)
                } else {
                    logDebug("No chain certificate found with alias $chainAlias", TAG)
                }
            } catch (e: Exception) {
                logError("deleteCertificateWithChainFromKeyStore Exception: ${e.message}", e, TAG)
            }
        }
    }

    private fun decodeCertificate(certStr: String, factory: CertificateFactory): X509Certificate {
        val cleanedCertificate = certStr
            .replace("-----BEGIN CERTIFICATE-----", "")
            .replace("-----END CERTIFICATE-----", "")
            .replace("\\s".toRegex(), "")
        val certificateData = Base64.decode(cleanedCertificate, Base64.DEFAULT)
        return factory.generateCertificate(certificateData.inputStream()) as X509Certificate
    }

    private fun PKCS10CertificationRequest.toPEM(): String {
        val sw = StringWriter()
        PemWriter(sw).use { writer ->
            writer.writeObject(PemObject("CERTIFICATE REQUEST", this.encoded))
        }
        return sw.toString()
    }

    private fun getInitializedCipherForEncryption(): Cipher {
        val cipher = getSymmetricCipher()
        val secretKey = getOrCreateMasterSecretKey()
        cipher.init(Cipher.ENCRYPT_MODE, secretKey)
        return cipher
    }

    private fun getInitializedCipherForDecryption(initializationVector: ByteArray): Cipher {
        val cipher = getSymmetricCipher()
        val secretKey = getOrCreateMasterSecretKey()
        cipher.init(Cipher.DECRYPT_MODE, secretKey, GCMParameterSpec(128, initializationVector))
        return cipher
    }

    private fun getSymmetricCipher(): Cipher {
        val transformation = "$ENCRYPTION_ALGORITHM/$ENCRYPTION_BLOCK_MODE/$ENCRYPTION_PADDING"
        return Cipher.getInstance(transformation)
    }

    private fun getOrCreateMasterSecretKey(): SecretKey {
        keyStore.getKey(MASTER_SYMMETRIC_KEY_ALIAS, null)?.let {
            return it as SecretKey
        }
        // if you reach here, then a new SecretKey must be generated for that keyName
        val purpose = KeyProperties.PURPOSE_ENCRYPT or KeyProperties.PURPOSE_DECRYPT
        val paramsBuilder = KeyGenParameterSpec.Builder(MASTER_SYMMETRIC_KEY_ALIAS, purpose).apply {
            setBlockModes(ENCRYPTION_BLOCK_MODE)
            setEncryptionPaddings(ENCRYPTION_PADDING)
            setKeySize(SYMMETRIC_KEY_SIZE)

            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.P) {
                if (context.packageManager.hasSystemFeature(PackageManager.FEATURE_STRONGBOX_KEYSTORE)) {
                    setIsStrongBoxBacked(true)
                }
            }
        }
        val keyGenParams = paramsBuilder.build()
        val keyGenerator =
            KeyGenerator.getInstance(KeyProperties.KEY_ALGORITHM_AES, ANDROID_KEYSTORE)
        keyGenerator.init(keyGenParams)
        return keyGenerator.generateKey()
    }

    private fun getOrCreateBiometricSecretKey(): SecretKey {
        keyStore.getKey(BIOMETRIC_KEY_ALIAS, null)?.let {
            return it as SecretKey
        }
        // if you reach here, then a new SecretKey must be generated for that keyName
        val purpose = KeyProperties.PURPOSE_ENCRYPT or KeyProperties.PURPOSE_DECRYPT
        val paramsBuilder = KeyGenParameterSpec.Builder(BIOMETRIC_KEY_ALIAS, purpose).apply {
            setBlockModes(ENCRYPTION_BLOCK_MODE)
            setEncryptionPaddings(ENCRYPTION_PADDING)
            setKeySize(SYMMETRIC_KEY_SIZE)
            setUserAuthenticationRequired(true)
            @Suppress("DEPRECATION")
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.R) {
                setUserAuthenticationParameters(
                    0,
                    KeyProperties.AUTH_BIOMETRIC_STRONG or
                            KeyProperties.AUTH_DEVICE_CREDENTIAL
                )
            } else {
                // Deprecated but do not have any alternative for lower APIs as usual.
                setUserAuthenticationValidityDurationSeconds(-1)
            }
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.P) {
                setUnlockedDeviceRequired(true)
                if (context.packageManager.hasSystemFeature(PackageManager.FEATURE_STRONGBOX_KEYSTORE)) {
                    setIsStrongBoxBacked(true)
                }
            }
            setInvalidatedByBiometricEnrollment(true)
        }
        val keyGenParams = paramsBuilder.build()
        val keyGenerator =
            KeyGenerator.getInstance(KeyProperties.KEY_ALGORITHM_AES, ANDROID_KEYSTORE)
        keyGenerator.init(keyGenParams)
        return keyGenerator.generateKey()
    }

    private fun generateSelfSignedCertificateSecret(
        host: String,
        keyPair: KeyPair,
    ): Certificate {
        val subject = X500Principal("CN=$host")

        val notBefore = System.currentTimeMillis()
        val notAfter = notBefore + (1000L * 3600L * 24 * 365 * 30)

        val encodableAltNames: Array<ASN1Encodable> =
            arrayOf(GeneralName(GeneralName.dNSName, host))
        val purposes = arrayOf(KeyPurposeId.id_kp_serverAuth, KeyPurposeId.id_kp_clientAuth)

        val certBuilder: X509v3CertificateBuilder = JcaX509v3CertificateBuilder(
            subject,
            BigInteger.ONE, Date(notBefore), Date(notAfter), subject, keyPair.public
        )

        try {
            certBuilder.addExtension(Extension.basicConstraints, true, BasicConstraints(false))
            certBuilder.addExtension(
                Extension.keyUsage,
                true,
                KeyUsage(KeyUsage.digitalSignature + KeyUsage.keyEncipherment)
            )
            certBuilder.addExtension(Extension.extendedKeyUsage, false, ExtendedKeyUsage(purposes))
            certBuilder.addExtension(
                Extension.subjectAlternativeName,
                false,
                DERSequence(encodableAltNames)
            )

            val signer = JcaContentSignerBuilder("SHA256withRSAandMGF1").setProvider(
                BouncyCastleProvider.PROVIDER_NAME
            ).build(keyPair.private)
            val certHolder = certBuilder.build(signer)

            return JcaX509CertificateConverter().setProvider(
                BouncyCastleProvider.PROVIDER_NAME
            ).getCertificate(certHolder)
        } catch (e: java.lang.Exception) {
            throw AssertionError(e.message)
        }
    }

    override fun getPrivateKey(alias: String): PrivateKey? {
        return when (val privateKeyEntry = keyStore.getEntry(alias, null)) {
            is PrivateKeyEntry -> privateKeyEntry.privateKey
            else -> null
        }
    }

    override fun getCertificate(alias: String): Certificate? = keyStore.getCertificate(alias)

    override fun getCertificateChain(alias: String, chainCount: Int): Array<Certificate> {
        val certificates = ArrayList<Certificate>()
        for (index in 0 until chainCount) {
            certificates.add(keyStore.getCertificate("$alias-chain-$index"))
        }

        return certificates.toTypedArray()
    }

    override fun hasAlias(alias: String): Boolean = keyStore.containsAlias(alias)

    override fun deletePrivateKey(alias: String) = keyStore.deleteEntry(alias)
}
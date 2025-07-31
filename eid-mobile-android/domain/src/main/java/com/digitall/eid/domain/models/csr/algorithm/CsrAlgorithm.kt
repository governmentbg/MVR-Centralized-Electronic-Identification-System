package com.digitall.eid.domain.models.csr.algorithm

import com.digitall.eid.domain.models.base.TypeEnum

enum class CsrAlgorithm(override val type: String, val keySize: Int) : TypeEnum {
    RSA_1024(keySize = 1024, type = "rsa_1024"),
    RSA_2048(keySize = 2048, type = "rsa_2048"),
    RSA_3072(keySize = 3072, type = "rsa_3072"),
    SPEC_192_R1(keySize = 192, type = "secp192r1"),
    SPEC_224_R1(keySize = 224, type = "secp224r1"),
    SPEC_256_R1(keySize = 256, type = "spec256r1"),
    SPEC_384_R1(keySize = 384, type = "secp384r1"),
    SPEC_521_R1(keySize = 521, type = "secp521r1"),
    P_224(keySize = 224, type = "p-224"),
    P_256(keySize = 256, type = "p-256"),
    P_384(keySize = 384, type = "p-384"),
    P_512(keySize = 512, type = "p-521"),
}
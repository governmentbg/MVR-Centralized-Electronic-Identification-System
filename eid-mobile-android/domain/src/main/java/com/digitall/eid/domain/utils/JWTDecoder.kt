/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.utils

import com.digitall.eid.domain.models.user.UserModel

interface JWTDecoder {

    fun getUser(fromToken: String): UserModel?

}
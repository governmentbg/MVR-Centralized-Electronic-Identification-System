/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.common

data class MockResponse(
    val isEnabled: Boolean,
    val body: String,
    val message: String,
    val serverCode: Int,
    val contentType: String = "application/json"
)
/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.models.network.base

class EmptyResponse

@Suppress("UNCHECKED_CAST")
fun <T> getEmptyResponse(): T {
    return EmptyResponse() as T
}
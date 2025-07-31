/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.models.common

interface DiffEquals {

    fun isItemSame(other: Any?): Boolean

    fun isContentSame(other: Any?): Boolean

}
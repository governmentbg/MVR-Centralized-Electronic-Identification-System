/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.repository.local.base

interface BaseLocalRepositoryWithDataMethods<T> : BaseLocalRepository<T> {

    fun addAll(data: T)

}
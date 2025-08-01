/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.base

abstract class BaseMapperWithData<From, Data, To> {

    abstract fun map(from: From, data: Data?): To

    open fun mapList(fromList: List<From>, data: Data?): List<To> {
        return fromList.map { map(it, data) }
    }

}
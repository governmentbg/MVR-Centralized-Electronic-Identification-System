/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.mappers.empowerment.common.all

import com.digitall.eid.data.mappers.base.BaseReverseMapper
import com.digitall.eid.data.utils.StrictMapperConfig
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentSortingModel
import com.digitall.eid.models.empowerment.common.all.EmpowermentSortingModelUi
import org.mapstruct.Mapper
import org.mapstruct.factory.Mappers

class EmpowermentSortingModelUiMapper :
    BaseReverseMapper<EmpowermentSortingModel, EmpowermentSortingModelUi>() {

    @Mapper(config = StrictMapperConfig::class)
    interface ModelMapper {
        fun map(from: EmpowermentSortingModel): EmpowermentSortingModelUi
        fun reverse(to: EmpowermentSortingModelUi): EmpowermentSortingModel
    }

    override fun map(from: EmpowermentSortingModel): EmpowermentSortingModelUi {
        return Mappers.getMapper(ModelMapper::class.java).map(from)
    }

    override fun reverse(to: EmpowermentSortingModelUi): EmpowermentSortingModel {
        return Mappers.getMapper(ModelMapper::class.java).reverse(to)
    }


}
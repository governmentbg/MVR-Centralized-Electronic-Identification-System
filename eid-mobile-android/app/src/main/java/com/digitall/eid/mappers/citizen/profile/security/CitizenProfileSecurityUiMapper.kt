package com.digitall.eid.mappers.citizen.profile.security

import com.digitall.eid.data.mappers.base.BaseMapperWithData
import com.digitall.eid.domain.models.user.UserAcrEnum
import com.digitall.eid.models.citizen.profile.security.CitizenProfileSecurityAdapterMarker
import com.digitall.eid.models.citizen.profile.security.CitizenProfileSecurityElementsEnumUi
import com.digitall.eid.models.citizen.profile.security.CitizenProfileSecurityUiModel
import com.digitall.eid.models.list.CommonTitleCheckboxUi

class CitizenProfileSecurityUiMapper: BaseMapperWithData<CitizenProfileSecurityUiModel, UserAcrEnum, List<CitizenProfileSecurityAdapterMarker>>() {

    override fun map(from: CitizenProfileSecurityUiModel, data: UserAcrEnum?) = buildList {
        add(
            CommonTitleCheckboxUi(
                title = CitizenProfileSecurityElementsEnumUi.MULTI_FACTOR_AUTHENTICATION_CHECKBOX.title,
                elementEnum = CitizenProfileSecurityElementsEnumUi.MULTI_FACTOR_AUTHENTICATION_CHECKBOX,
                isChecked = from.isTwoFactorEnabled
            )
        )
        if (data == UserAcrEnum.LOW) {
            add(
                CommonTitleCheckboxUi(
                    title = CitizenProfileSecurityElementsEnumUi.PROFILE_SECURITY_PIN_CHECKBOX.title,
                    elementEnum = CitizenProfileSecurityElementsEnumUi.PROFILE_SECURITY_PIN_CHECKBOX,
                    isChecked = from.isPinEnabled
                )
            )
            add(
                CommonTitleCheckboxUi(
                    title = CitizenProfileSecurityElementsEnumUi.PROFILE_SECURITY_BIOMETRICS_CHECKBOX.title,
                    elementEnum = CitizenProfileSecurityElementsEnumUi.PROFILE_SECURITY_BIOMETRICS_CHECKBOX,
                    isChecked = from.isBiometricEnabled,
                    isEnabled = from.isBiometricAvailable,
                )
            )
        }
    }
}
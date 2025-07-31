/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.mappers.common

import com.digitall.eid.R
import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.authentication.AuthCreatePassCodeResponseErrorCodes
import com.digitall.eid.utils.CurrentContext
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

class CreateCodeResponseErrorToStringMapper :
    BaseMapper<AuthCreatePassCodeResponseErrorCodes, String>(), KoinComponent {

    private val currentContext: CurrentContext by inject()

    override fun map(from: AuthCreatePassCodeResponseErrorCodes): String {
        return currentContext.get().getString(
            when (from) {
                AuthCreatePassCodeResponseErrorCodes.PASS_CODE_CONTAINS_PHONE_NUMBER ->
                    R.string.create_pin_error_pass_code_your_part_phone_number

                AuthCreatePassCodeResponseErrorCodes.PASS_CODE_IS_SEQUENT_DIGITS ->
                    R.string.create_pin_error_pass_code_cannot_be_set_as_sequential_numbers

                AuthCreatePassCodeResponseErrorCodes.PASS_CODE_HAS_REPEATED_DIGITS ->
                    R.string.create_pin_error_pass_code_repeating_digits_more_than_2

                AuthCreatePassCodeResponseErrorCodes.PASS_CODE_HAS_DATE_OF_BIRTH ->
                    R.string.create_pin_error_pass_code_your_date_of_birth

                AuthCreatePassCodeResponseErrorCodes.PASS_CODE_ALREADY_USED ->
                    R.string.create_pin_error_pass_code_already_used
            }
        )
    }
}
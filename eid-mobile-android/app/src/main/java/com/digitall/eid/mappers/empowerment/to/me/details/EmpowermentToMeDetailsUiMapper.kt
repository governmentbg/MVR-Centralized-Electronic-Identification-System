/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.mappers.empowerment.to.me.details

import com.digitall.eid.R
import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.domain.UiDateFormats
import com.digitall.eid.domain.extensions.fromServerDateToTextDate
import com.digitall.eid.domain.extensions.getEnumValue
import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentItem
import com.digitall.eid.extensions.notEmpty
import com.digitall.eid.models.common.ButtonColorUi
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.empowerment.common.details.EmpowermentDenialReasonEnum
import com.digitall.eid.models.empowerment.common.details.EmpowermentDetailsAdapterMarker
import com.digitall.eid.models.empowerment.common.details.EmpowermentDetailsElementsEnumUi
import com.digitall.eid.models.empowerment.common.details.EmpowermentDetailsStatementsElementsUi
import com.digitall.eid.models.empowerment.common.filter.EmpowermentFilterStatusEnumUi
import com.digitall.eid.models.empowerment.common.filter.EmpowermentOnBehalfOf
import com.digitall.eid.models.list.CommonButtonUi
import com.digitall.eid.models.list.CommonEmptySpaceSizeEnum
import com.digitall.eid.models.list.CommonEmptySpaceUi
import com.digitall.eid.models.list.CommonLabeledSimpleTextUi
import com.digitall.eid.models.list.CommonSeparatorUi
import com.digitall.eid.models.list.CommonSimpleTextInFieldUi
import com.digitall.eid.models.list.CommonSimpleTextUi
import com.digitall.eid.models.list.CommonTitleSmallInFieldUi
import com.digitall.eid.models.list.CommonTitleSmallUi
import com.digitall.eid.models.list.CommonTitleUi
import com.digitall.eid.utils.translateUidType

class EmpowermentToMeDetailsUiMapper :
    BaseMapper<EmpowermentItem, List<EmpowermentDetailsAdapterMarker>>() {

    override fun map(
        from: EmpowermentItem,
    ): List<EmpowermentDetailsAdapterMarker> {
        return with(from) {
            val status = getEnumValue<EmpowermentFilterStatusEnumUi>(from.calculatedStatusOn ?: "")
            val onBehalfOf = getEnumValue<EmpowermentOnBehalfOf>(from.onBehalfOf ?: "")
            buildList {
                when (status) {
                    EmpowermentFilterStatusEnumUi.CREATED ->
                        add(
                            CommonTitleUi(
                                title = StringSource(R.string.empowerment_details_signing_title),
                            )
                        )

                    else ->
                        add(
                            CommonTitleUi(
                                title = StringSource(R.string.empowerment_details_preview_title),
                            )
                        )
                }
                add(CommonSeparatorUi())
                add(
                    CommonTitleSmallUi(
                        title = StringSource(R.string.empowerment_details_title)
                    )
                )
                add(
                    CommonSimpleTextUi(
                        title = StringSource(R.string.empowerment_details_number_title),
                        text = StringSource(number),
                    )
                )
                status?.title?.let { statusName ->
                    add(
                        CommonSimpleTextUi(
                            text = statusName,
                            title = StringSource(R.string.empowerment_details_status_title),
                            colorRes = status.colorRes,
                            iconResLeft = status.iconRes,
                        )
                    )
                }
                denialReason?.let { reason ->
                    val denialReasonEnum = getEnumValue<EmpowermentDenialReasonEnum>(reason)
                        ?: EmpowermentDenialReasonEnum.NONE
                    if (denialReasonEnum != EmpowermentDenialReasonEnum.NONE) {
                        add(
                            CommonSimpleTextUi(
                                title = StringSource(R.string.empowerment_details_reason_title),
                                text = denialReasonEnum.title,
                                colorRes = R.color.color_BF1212,
                                maxLines = 24,
                            )
                        )
                    }
                }
                if (!empowermentWithdrawals.isNullOrEmpty()) {
                    empowermentWithdrawals?.filter {
                        !it.reason.isNullOrEmpty()
                    }?.mapNotNull {
                        it.reason
                    }?.let {
                        add(
                            CommonSimpleTextUi(
                                title = StringSource(R.string.empowerment_details_reason_title),
                                text = StringSource(it.joinToString(separator = ",\n")),
                                colorRes = R.color.color_BF1212,
                                maxLines = 24,
                            )
                        )
                    }
                }
                if (!empowermentDisagreements.isNullOrEmpty()) {
                    empowermentDisagreements?.filter {
                        !it.reason.isNullOrEmpty()
                    }?.mapNotNull {
                        it.reason
                    }?.let {
                        add(
                            CommonSimpleTextUi(
                                title = StringSource(R.string.empowerment_details_reason_title),
                                text = StringSource(it.joinToString(separator = ",\n")),
                                colorRes = R.color.color_BF1212,
                                maxLines = 24,
                            )
                        )
                    }
                }
                startDate?.fromServerDateToTextDate(
                    dateFormat = UiDateFormats.WITH_TIME,
                )?.notEmpty {
                    add(
                        CommonSimpleTextUi(
                            title = StringSource(R.string.empowerment_details_start_date_title),
                            text = StringSource(it),
                        )
                    )
                }
                add(
                    CommonSimpleTextUi(
                        title = StringSource(R.string.empowerment_details_end_date_title),
                        text = if (expiryDate.isNullOrEmpty())
                            StringSource(R.string.empowerment_details_indefinitely)
                        else StringSource(
                            expiryDate?.fromServerDateToTextDate(
                                dateFormat = UiDateFormats.WITH_TIME,
                            )
                        )
                    )
                )
                if (!uid.isNullOrEmpty()) {
                    when (onBehalfOf) {
                        EmpowermentOnBehalfOf.INDIVIDUAL -> {
                            add(
                                CommonSimpleTextUi(
                                    title = StringSource(R.string.empowerment_details_onbehalfof_title),
                                    text = StringSource(R.string.empowerment_details_onbehalfof_individual_enum_type),
                                )
                            )
                            uid?.notEmpty { uid ->
                                add(
                                    CommonSimpleTextUi(
                                        title = StringSource(R.string.empowerment_details_from_id_individual_title),
                                        text = StringSource(uid),
                                    )
                                )
                            }
                        }

                        EmpowermentOnBehalfOf.LEGAL_ENTITY -> {
                            add(
                                CommonSimpleTextUi(
                                    title = StringSource(R.string.empowerment_details_onbehalfof_title),
                                    text = StringSource(R.string.empowerment_details_onbehalfof_legal_entity_enum_type),
                                )
                            )
                            uid?.notEmpty { uid ->
                                add(
                                    CommonSimpleTextUi(
                                        title = StringSource(R.string.empowerment_details_from_id_legal_entity_title),
                                        text = StringSource(uid),
                                    )
                                )
                            }
                        }

                        else -> {
                            // NO
                        }
                    }
                }
                name?.notEmpty { name ->
                    add(
                        CommonSimpleTextUi(
                            title = when (onBehalfOf) {
                                EmpowermentOnBehalfOf.LEGAL_ENTITY -> StringSource(R.string.empowerment_details_legal_entity_name_title)
                                else -> StringSource(R.string.empowerment_details_name_title)
                            },
                            text = StringSource(name),
                        )
                    )
                }

                val authorizedUids = authorizerUids?.filter {
                    !it.uid.isNullOrEmpty() && !it.uidType.isNullOrEmpty()
                }?.map {
                    translateUidType(
                        uidType = it.uidType,
                        uidEmpowerer = it.uid,
                        nameEmpowerer = it.name,
                    )
                }
                if (authorizedUids.isNullOrEmpty().not()) {
                    add(
                        CommonSimpleTextUi(
                            title = StringSource(R.string.empowerment_details_legal_representatives_title),
                            text = StringSource(
                                sources = authorizedUids ?: emptyList(),
                                separator = "\n"
                            ),
                            maxLines = (authorizedUids?.size ?: 0) * 2
                        )
                    )
                }

                val empoweredUidsStrings = empoweredUids?.filter {
                    !it.uid.isNullOrEmpty() && !it.uidType.isNullOrEmpty()
                }?.map {
                    translateUidType(
                        uidType = it.uidType,
                        uidEmpowerer = it.uid,
                        nameEmpowerer = it.name,
                    )
                }
                if (empoweredUidsStrings.isNullOrEmpty().not()) {
                    add(
                        CommonSimpleTextUi(
                            title = StringSource(R.string.empowerment_details_empowered_people_ids_title),
                            text = StringSource(
                                sources = empoweredUidsStrings ?: emptyList(),
                                separator = "\n"
                            ),
                            maxLines = (empoweredUidsStrings?.size ?: 0) * 2
                        )
                    )

                    if ((empoweredUidsStrings?.size ?: 0) > 1) {
                        add(
                            CommonLabeledSimpleTextUi(
                                title = StringSource(R.string.empowerment_details_empowerment_type_title),
                                labeledText = StringSource(R.string.empowerment_details_empowerment_type_together_title),
                                text = StringSource(R.string.empowerment_details_empowerment_type_together_description),
                                iconResLeft = R.drawable.ic_caution
                            )
                        )
                    }
                }
                providerName?.notEmpty { providerName ->
                    add(
                        CommonSimpleTextUi(
                            title = StringSource(R.string.empowerment_details_services_provider_title),
                            text = StringSource(providerName),
                        )
                    )
                }
                add(
                    CommonSimpleTextUi(
                        title = StringSource(R.string.empowerment_details_service_title),
                        text = StringSource("$serviceId - $serviceName"),
                    )
                )
                if (!volumeOfRepresentation.isNullOrEmpty()) {
                    volumeOfRepresentation?.filter {
                        !it.name.isNullOrEmpty()
                    }?.mapNotNull {
                        it.name
                    }?.let {
                        add(
                            CommonSimpleTextUi(
                                title = StringSource(R.string.empowerment_details_scope_title),
                                text = StringSource(it.joinToString(separator = ",\n")),
                                maxLines = 24,
                            ),
                        )
                    }
                }
                add(
                    CommonEmptySpaceUi(
                        size = CommonEmptySpaceSizeEnum.SIZE_4,
                    )
                )
                add(
                    CommonTitleSmallInFieldUi(
                        title = StringSource(R.string.empowerment_details_empowerment_history_title)
                    )
                )
                statusHistory?.filter {
                    !it.id.isNullOrEmpty() &&
                            !it.status.isNullOrEmpty() &&
                            !it.dateTime.isNullOrEmpty()
                }?.map {
                    (getEnumValue<EmpowermentDetailsStatementsElementsUi>(it.status ?: "")
                        ?: EmpowermentDetailsStatementsElementsUi.NONE) to it.dateTime
                }?.filter { element ->
                    listOf(
                        EmpowermentDetailsStatementsElementsUi.NONE,
                        EmpowermentDetailsStatementsElementsUi.COLLECTING_WITHDRAWAL_SIGNATURES,
                        EmpowermentDetailsStatementsElementsUi.AWAITING_SIGNATURE,
                    ).contains(element.first).not()
                }?.forEach { status ->
                    when (status.first) {
                        EmpowermentDetailsStatementsElementsUi.COLLECTING_AUTHORIZER_SIGNATURES -> {
                            empowermentSignatures?.forEach { empowermentSignature ->
                                val signerModel =
                                    authorizerUids?.first { element -> element.uid == empowermentSignature.signerUid }
                                signerModel?.let { signer ->
                                    status.second?.fromServerDateToTextDate(
                                        dateFormat = UiDateFormats.WITH_TIME,
                                    )?.notEmpty { dateTime ->
                                        add(
                                            CommonSimpleTextInFieldUi(
                                                title = status.first.title,
                                                text = StringSource("$dateTime ${signer.name}"),
                                            )
                                        )
                                    }
                                }
                            }
                        }

                        EmpowermentDetailsStatementsElementsUi.DISAGREEMENT_DECLARED -> {
                            empowermentDisagreements?.forEach { disagreement ->
                                val disagreementPerson = empoweredUids?.first { element -> element.uid == disagreement.issuerUid }
                                disagreementPerson?.let { person ->
                                    status.second?.fromServerDateToTextDate(
                                        dateFormat = UiDateFormats.WITH_TIME,
                                    )?.notEmpty { dateTime ->
                                        add(
                                            CommonSimpleTextInFieldUi(
                                                title = status.first.title,
                                                text = StringSource("$dateTime ${person.name}"),
                                            )
                                        )
                                    }
                                }
                            }
                        }

                        else -> status.second?.fromServerDateToTextDate(
                            dateFormat = UiDateFormats.WITH_TIME,
                        )?.notEmpty { dateTime ->
                            add(
                                CommonSimpleTextInFieldUi(
                                    title = status.first.title,
                                    text = StringSource(dateTime),
                                )
                            )
                        }
                    }
                }
                add(
                    CommonEmptySpaceUi(
                        size = CommonEmptySpaceSizeEnum.SIZE_4,
                    )
                )
                add(
                    CommonButtonUi(
                        title = StringSource(R.string.back),
                        elementEnum = EmpowermentDetailsElementsEnumUi.BUTTON_BACK,
                        buttonColor = ButtonColorUi.TRANSPARENT,
                    )
                )
            }
        }
    }

}
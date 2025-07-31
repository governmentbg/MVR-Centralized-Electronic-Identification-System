/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.di

import com.digitall.eid.ui.common.list.CommonButtonDelegate
import com.digitall.eid.ui.common.list.CommonButtonTransparentDelegate
import com.digitall.eid.ui.common.list.CommonCheckBoxDelegate
import com.digitall.eid.ui.common.list.CommonDatePickerDelegate
import com.digitall.eid.ui.common.list.CommonDialogWithSearchDelegate
import com.digitall.eid.ui.common.list.CommonDialogWithSearchMultiselectDelegate
import com.digitall.eid.ui.common.list.CommonDisclaimerTextDelegate
import com.digitall.eid.ui.common.list.CommonDoubleButtonDelegate
import com.digitall.eid.ui.common.list.CommonEditTextDelegate
import com.digitall.eid.ui.common.list.CommonEmptySpaceDelegate
import com.digitall.eid.ui.common.list.CommonLabeledSimpleTextDelegate
import com.digitall.eid.ui.common.list.CommonPhoneTextDelegate
import com.digitall.eid.ui.common.list.CommonSeparatorDelegate
import com.digitall.eid.ui.common.list.CommonSeparatorInFieldDelegate
import com.digitall.eid.ui.common.list.CommonSimpleInFieldTextDelegate
import com.digitall.eid.ui.common.list.CommonSimpleTextDelegate
import com.digitall.eid.ui.common.list.CommonSimpleTextExpiringDelegate
import com.digitall.eid.ui.common.list.CommonSpinnerDelegate
import com.digitall.eid.ui.common.list.CommonTextFieldDelegate
import com.digitall.eid.ui.common.list.CommonTextFieldMultipleDelegate
import com.digitall.eid.ui.common.list.CommonTitleBigDelegate
import com.digitall.eid.ui.common.list.CommonTitleCheckboxDelegate
import com.digitall.eid.ui.common.list.CommonTitleDelegate
import com.digitall.eid.ui.common.list.CommonTitleDescriptionDelegate
import com.digitall.eid.ui.common.list.CommonTitleSmallDelegate
import com.digitall.eid.ui.common.list.CommonTitleSmallInFieldDelegate
import com.digitall.eid.ui.fragments.applications.show.all.list.ApplicationsDelegate
import com.digitall.eid.ui.fragments.certificates.all.list.CertificatesDelegate
import com.digitall.eid.ui.fragments.common.search.multiselect.list.CommonBottomSheetWithSearchMultiselectDelegate
import com.digitall.eid.ui.fragments.common.search.normal.list.CommonBottomSheetWithSearchDelegate
import com.digitall.eid.ui.fragments.empowerment.from.me.all.list.EmpowermentFromMeDelegate
import com.digitall.eid.ui.fragments.empowerment.legal.all.list.EmpowermentLegalDelegate
import com.digitall.eid.ui.fragments.empowerment.to.me.all.list.EmpowermentToMeDelegate
import com.digitall.eid.ui.fragments.journal.from.me.all.list.JournalFromMeDelegate
import com.digitall.eid.ui.fragments.journal.to.me.all.list.JournalToMeDelegate
import com.digitall.eid.ui.fragments.main.tabs.more.list.TabMoreItemDelegate
import com.digitall.eid.ui.fragments.main.tabs.more.list.TabMoreSeparatorDelegate
import com.digitall.eid.ui.fragments.main.tabs.more.list.TabMoreTitleDelegate
import com.digitall.eid.ui.fragments.main.tabs.requests.list.TabRequestsDelegate
import com.digitall.eid.ui.fragments.notifications.channels.list.NotificationChannelDelegate
import com.digitall.eid.ui.fragments.notifications.notifications.list.NotificationChildDelegate
import com.digitall.eid.ui.fragments.notifications.notifications.list.NotificationParentDelegate
import com.digitall.eid.ui.fragments.payments.history.list.PaymentsHistoryDelegate
import org.koin.dsl.module

val delegatesModule = module {

    single<TabMoreItemDelegate> {
        TabMoreItemDelegate()
    }

    single<TabMoreTitleDelegate> {
        TabMoreTitleDelegate()
    }

    single<NotificationChannelDelegate> {
        NotificationChannelDelegate()
    }

    single<NotificationParentDelegate> {
        NotificationParentDelegate()
    }

    single<NotificationChildDelegate> {
        NotificationChildDelegate()
    }

    single<CommonEditTextDelegate> {
        CommonEditTextDelegate()
    }

    single<CommonTitleDelegate> {
        CommonTitleDelegate()
    }

    single<CommonSeparatorDelegate> {
        CommonSeparatorDelegate()
    }

    single<CommonButtonTransparentDelegate> {
        CommonButtonTransparentDelegate()
    }

    single<CommonTextFieldDelegate> {
        CommonTextFieldDelegate()
    }

    single<CommonTitleSmallDelegate> {
        CommonTitleSmallDelegate()
    }

    single<CommonButtonDelegate> {
        CommonButtonDelegate()
    }

    single<CommonBottomSheetWithSearchDelegate> {
        CommonBottomSheetWithSearchDelegate()
    }

    single<CommonBottomSheetWithSearchMultiselectDelegate> {
        CommonBottomSheetWithSearchMultiselectDelegate()
    }

    single<EmpowermentFromMeDelegate> {
        EmpowermentFromMeDelegate()
    }

    single<EmpowermentToMeDelegate> {
        EmpowermentToMeDelegate()
    }

    single<EmpowermentLegalDelegate> {
        EmpowermentLegalDelegate()
    }

    single<CommonDoubleButtonDelegate> {
        CommonDoubleButtonDelegate()
    }

    single<CommonDisclaimerTextDelegate> {
        CommonDisclaimerTextDelegate()
    }

    single<CommonCheckBoxDelegate> {
        CommonCheckBoxDelegate()
    }

    single<CommonDatePickerDelegate> {
        CommonDatePickerDelegate()
    }

    single<CommonDialogWithSearchDelegate> {
        CommonDialogWithSearchDelegate()
    }

    single<CommonDialogWithSearchMultiselectDelegate> {
        CommonDialogWithSearchMultiselectDelegate()
    }

    single<CommonSpinnerDelegate> {
        CommonSpinnerDelegate()
    }

    single<CommonTitleBigDelegate> {
        CommonTitleBigDelegate()
    }

    single<CommonSimpleTextDelegate> {
        CommonSimpleTextDelegate()
    }

    single<CommonSimpleInFieldTextDelegate> {
        CommonSimpleInFieldTextDelegate()
    }

    single<CommonTitleSmallInFieldDelegate> {
        CommonTitleSmallInFieldDelegate()
    }

    single<CommonSeparatorInFieldDelegate> {
        CommonSeparatorInFieldDelegate()
    }

    single<CommonEmptySpaceDelegate> {
        CommonEmptySpaceDelegate()
    }

    single<CommonTitleDescriptionDelegate> {
        CommonTitleDescriptionDelegate()
    }

    single<CommonLabeledSimpleTextDelegate> {
        CommonLabeledSimpleTextDelegate()
    }

    single<CommonSimpleTextExpiringDelegate> {
        CommonSimpleTextExpiringDelegate()
    }

    single<CommonTextFieldMultipleDelegate> {
        CommonTextFieldMultipleDelegate()
    }

    single<CommonPhoneTextDelegate> {
        CommonPhoneTextDelegate()
    }

    single<JournalToMeDelegate> {
        JournalToMeDelegate()
    }

    single<JournalFromMeDelegate> {
        JournalFromMeDelegate()
    }

    single<ApplicationsDelegate> {
        ApplicationsDelegate()
    }

    single<CertificatesDelegate> {
        CertificatesDelegate()
    }

    single<TabMoreSeparatorDelegate> {
        TabMoreSeparatorDelegate()
    }

    single<TabRequestsDelegate> {
        TabRequestsDelegate()
    }

    single<PaymentsHistoryDelegate> {
        PaymentsHistoryDelegate()
    }

    single<CommonTitleCheckboxDelegate> {
        CommonTitleCheckboxDelegate()
    }

}
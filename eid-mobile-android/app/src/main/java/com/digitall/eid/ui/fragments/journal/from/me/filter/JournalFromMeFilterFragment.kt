package com.digitall.eid.ui.fragments.journal.from.me.filter

import androidx.navigation.fragment.navArgs
import com.digitall.eid.R
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonDialogWithSearchMultiselectUi
import com.digitall.eid.ui.fragments.journal.base.filter.BaseJournalFilterFragment
import com.digitall.eid.ui.fragments.journal.from.me.filter.list.JournalFromMeFilterAdapter
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class JournalFromMeFilterFragment : BaseJournalFilterFragment<JournalFromMeFilterViewModel>() {

    companion object {
        private const val TAG = "JournalFromMeFilterFragmentTag"
    }

    override val viewModel: JournalFromMeFilterViewModel by viewModel()
    override val adapter: JournalFromMeFilterAdapter by inject()
    private val args: JournalFromMeFilterFragmentArgs by navArgs()

    override fun parseArguments() {
        try {
            viewModel.setViewModel(filterModel = args.filter, localizations = args.localizations)
        } catch (exception: Exception) {
            logError(
                "parseArguments Exception: ${exception.message}",
                exception,
                TAG
            )
        }
    }

    override fun onDialogWithSearchMultiselectClicked(model: CommonDialogWithSearchMultiselectUi) {
        viewModel.showDialogWithSearchMultiselect(model)
    }

    override fun setupView() {
        super.setupView()
        binding.toolbar.setTitleText(StringSource(R.string.journals_from_me_title))
    }
}
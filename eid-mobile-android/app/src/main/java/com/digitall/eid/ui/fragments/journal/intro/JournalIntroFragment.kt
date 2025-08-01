/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.journal.intro

import com.digitall.eid.databinding.FragmentJournalIntroBinding
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.ui.fragments.base.BaseFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class JournalIntroFragment :
    BaseFragment<FragmentJournalIntroBinding, JournalIntroViewModel>() {

    companion object {
        private const val TAG = "JournalIntroFragmentTag"
    }

    override fun getViewBinding() = FragmentJournalIntroBinding.inflate(layoutInflater)

    override val viewModel: JournalIntroViewModel by viewModel()

    override fun setupControls() {
        binding.toolbar.navigationClickListener = {
            viewModel.onBackPressed()
        }
        binding.buttonFromMe.onClickThrottle {
            viewModel.toJournalFromMe()
        }
        binding.buttonToMe.onClickThrottle {
            viewModel.toJournalToMe()
        }
    }

}
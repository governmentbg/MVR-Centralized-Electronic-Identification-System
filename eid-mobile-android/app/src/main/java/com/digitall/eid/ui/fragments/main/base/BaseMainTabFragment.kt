/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.main.base

import androidx.viewbinding.ViewBinding
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.ui.fragments.base.BaseFragment

abstract class BaseMainTabFragment<VB : ViewBinding, VM : BaseViewModel> : BaseFragment<VB, VM>() {

    companion object {
        private const val TAG = "BaseMainTabFragmentTag"
    }

}
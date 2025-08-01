/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.common.list

import android.content.res.ColorStateList
import android.text.InputFilter
import android.text.InputType
import android.text.TextWatcher
import android.view.Gravity
import android.view.ViewGroup
import androidx.core.view.isVisible
import androidx.core.view.updateLayoutParams
import androidx.core.widget.doAfterTextChanged
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.R
import com.digitall.eid.databinding.ListItemEditTextBinding
import com.digitall.eid.domain.DELAY_250
import com.digitall.eid.extensions.hideKeyboard
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.extensions.noLeadingTrailingSpaces
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.extensions.onDonePressed
import com.digitall.eid.extensions.setBackgroundDrawableResource
import com.digitall.eid.extensions.setFocusChangeListener
import com.digitall.eid.extensions.showKeyboard
import com.digitall.eid.models.list.CommonEditTextInputType
import com.digitall.eid.models.list.CommonEditTextUi
import com.digitall.eid.models.list.CommonEditTextUiType
import com.digitall.eid.models.list.CommonListElementAdapterMarker
import com.google.android.material.textfield.TextInputLayout.END_ICON_CLEAR_TEXT
import com.google.android.material.textfield.TextInputLayout.END_ICON_PASSWORD_TOGGLE
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.Job
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch

class CommonEditTextDelegate : CommonInputDelegate() {

    var doneListener: ((model: CommonEditTextUi) -> Unit)? = null
    var changeListener: ((model: CommonEditTextUi) -> Unit)? = null
    var eraseClickListener: ((model: CommonEditTextUi) -> Unit)? = null
    var focusChangedListener: ((model: CommonEditTextUi) -> Unit)? = null
    var characterFilterListener: ((model: CommonEditTextUi, char: Char) -> Boolean)? = null

    override fun isForViewType(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int
    ): Boolean {
        return items[position] is CommonEditTextUi
    }

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemEditTextBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position] as CommonEditTextUi)
    }

    override fun onViewRecycled(holder: RecyclerView.ViewHolder) {
        super.onViewRecycled(holder)
        if (holder is ViewHolder) {
            holder.cancelDebounce()
        }
    }

    private inner class ViewHolder(
        private val binding: ListItemEditTextBinding,
    ) : RecyclerView.ViewHolder(binding.root) {

        private var currentItem: CommonEditTextUi? = null

        private var textWatcher: TextWatcher? = null

        private var debounceJob: Job? = null
        private val debouncePeriod = DELAY_250

        init {
            binding.etValue.onDonePressed {
                val itemModel = currentItem ?: return@onDonePressed
                if (binding.etValue.text.isNullOrEmpty().not()) {
                    val trimmedText = binding.etValue.text?.toString()?.noLeadingTrailingSpaces()
                    binding.etValue.setText(trimmedText)
                    binding.etValue.setSelection(trimmedText?.length ?: 0)
                }
                doneListener?.invoke(
                    itemModel.copy(
                        selectedValue = binding.etValue.text.toString()
                    )
                )
            }
            binding.etValue.setFocusChangeListener { hasFocus ->
                val itemModel = currentItem ?: return@setFocusChangeListener
                if (!hasFocus) {
                    binding.etValue.hideKeyboard()
                } else {
                    if (binding.etValue.text.isNullOrEmpty().not()) {
                        val trimmedText =
                            binding.etValue.text?.toString()?.noLeadingTrailingSpaces()
                        binding.etValue.setText(trimmedText)
                        binding.etValue.setSelection(trimmedText?.length ?: 0)
                    }
                    binding.etValue.showKeyboard()
                }
                val modifiedModel = itemModel.copy(
                    hasFocus = hasFocus,
                    selectedValue = binding.etValue.text.toString()
                )
                focusChangedListener?.invoke(
                    modifiedModel
                )
                currentItem = modifiedModel
            }
        }

        fun bind(model: CommonEditTextUi) {
            val wasFocusedAndHadWindow =
                binding.etValue.isFocused && binding.etValue.hasWindowFocus()
            val previousSelectionStart = binding.etValue.selectionStart
            val previousInputType = binding.etValue.inputType

            this.currentItem = model

            textWatcher?.let { binding.etValue.removeTextChangedListener(it) }

            if (binding.etValue.text.toString() != model.selectedValue) {
                binding.etValue.setText(model.selectedValue)
            }

            binding.tvTitle.text = model.title.getString(binding.root.context)
            binding.icQuestion.isVisible = model.question
            binding.icRequired.isVisible = model.required
            when (model.type) {
                CommonEditTextUiType.PASSWORD,
                CommonEditTextUiType.PASSWORD_NUMBERS -> {
                    binding.valueInputLayout.setEndIconStyle(
                        mode = END_ICON_PASSWORD_TOGGLE,
                        tintList = ColorStateList.valueOf(binding.root.context.getColor(R.color.color_0C53B2)),
                        drawable = R.drawable.toggle_password
                    )
                }

                else -> {
                    binding.valueInputLayout.setEndIconStyle(
                        mode = END_ICON_CLEAR_TEXT,
                        drawable = R.drawable.ic_clean
                    )
                }
            }

            model.prefix?.let {
                binding.valueInputLayout.prefixText = it.getString(binding.root.context)
                binding.valueInputLayout.prefixTextView.updateLayoutParams {
                    height = ViewGroup.LayoutParams.MATCH_PARENT
                }
                binding.valueInputLayout.prefixTextView.gravity = Gravity.CENTER
            }
            model.prefixTextColor?.let {
                binding.valueInputLayout.setPrefixTextColor(
                    ColorStateList.valueOf(
                        binding.root.context.getColor(
                            it
                        )
                    )
                )
            }
            model.suffix?.let {
                binding.valueInputLayout.suffixText = it.getString(binding.root.context)
                binding.valueInputLayout.suffixTextView.updateLayoutParams {
                    height = ViewGroup.LayoutParams.MATCH_PARENT
                }
                binding.valueInputLayout.suffixTextView.gravity = Gravity.CENTER
            }
            model.suffixTextColor?.let {
                binding.valueInputLayout.setSuffixTextColor(
                    ColorStateList.valueOf(
                        binding.root.context.getColor(
                            it
                        )
                    )
                )
            }
            binding.valueInputLayout.hint = model.hint?.getString(binding.root.context)
                ?: model.type.hint.getString(binding.root.context)

            if (!model.selectedValue.isNullOrEmpty()) {
                binding.etValue.setBackgroundDrawableResource(R.drawable.bg_text)
                binding.valueInputLayout.setHintTextColorResource(R.color.color_0C53B2)
            } else {
                binding.etValue.setBackgroundDrawableResource(R.drawable.bg_text_not_entered)
                binding.valueInputLayout.setHintTextColorResource(R.color.color_94A3B8)
            }

            binding.etValue.isEnabled = model.isEnabled
            binding.icErase.isVisible = model.hasEraseButton

            val newInputType = determineInputType(model)
            if (previousInputType != newInputType) {
                binding.etValue.inputType = newInputType
            }

            binding.etValue.filters = getFilters(model)

            if (model.hasEraseButton) {
                binding.icErase.onClickThrottle {
                    eraseClickListener?.invoke(
                        model.copy(
                            selectedValue = binding.etValue.text.toString()
                        )
                    )
                }
            } else {
                binding.icErase.setOnClickListener(null)
            }

            textWatcher = binding.etValue.doAfterTextChanged { editable ->
                val boundItem = currentItem ?: return@doAfterTextChanged
                if (!editable.isNullOrEmpty()) {
                    binding.etValue.setBackgroundDrawableResource(R.drawable.bg_text)
                    binding.valueInputLayout.setHintTextColorResource(R.color.color_0C53B2)
                } else {
                    binding.etValue.setBackgroundDrawableResource(R.drawable.bg_text_not_entered)
                    binding.valueInputLayout.setHintTextColorResource(R.color.color_94A3B8)
                }

                debounceJob?.cancel()
                debounceJob = CoroutineScope(Dispatchers.Main).launch {
                    delay(debouncePeriod)
                    if (this@ViewHolder.currentItem == boundItem && binding.etValue.text.toString() == editable.toString()) {
                        if (isCurrentViewHolderVisible(bindingAdapterPosition = bindingAdapterPosition) && boundItem.hasFocus) {
                            changeListener?.invoke(boundItem.copy(selectedValue = editable.toString()))
                        }
                    }
                }
            }
            binding.etValue.addTextChangedListener(textWatcher)

            if (wasFocusedAndHadWindow && binding.etValue.text.toString() == model.selectedValue) {
                if (previousSelectionStart <= binding.etValue.text.toString().length) {
                    binding.etValue.setSelection(previousSelectionStart)
                } else {
                    binding.etValue.setSelection(binding.etValue.text?.length ?: 0)
                }
            }

            binding.tvError.isVisible = model.validationError != null
            binding.tvError.text = model.validationError?.getString(binding.root.context)
        }

        fun cancelDebounce() {
            debounceJob?.cancel()
            debounceJob = null
        }

        private fun determineInputType(model: CommonEditTextUi): Int {
            return when (model.type.inputType) {
                CommonEditTextInputType.CHARS -> InputType.TYPE_CLASS_TEXT or
                        InputType.TYPE_TEXT_FLAG_AUTO_COMPLETE or
                        InputType.TYPE_TEXT_FLAG_AUTO_CORRECT

                CommonEditTextInputType.CHARS_ALL_CAPS -> InputType.TYPE_CLASS_TEXT or
                        InputType.TYPE_TEXT_FLAG_CAP_CHARACTERS

                CommonEditTextInputType.CHARS_CAP -> InputType.TYPE_CLASS_TEXT or
                        InputType.TYPE_TEXT_FLAG_AUTO_COMPLETE or
                        InputType.TYPE_TEXT_FLAG_AUTO_CORRECT or
                        InputType.TYPE_TEXT_FLAG_CAP_WORDS or
                        InputType.TYPE_TEXT_FLAG_CAP_SENTENCES

                CommonEditTextInputType.DIGITS -> InputType.TYPE_CLASS_NUMBER

                CommonEditTextInputType.PHONE -> InputType.TYPE_CLASS_PHONE

                CommonEditTextInputType.PASSWORD -> InputType.TYPE_CLASS_TEXT or
                        InputType.TYPE_TEXT_VARIATION_PASSWORD

                CommonEditTextInputType.PASSWORD_NUMBERS -> InputType.TYPE_CLASS_NUMBER or
                        InputType.TYPE_NUMBER_VARIATION_PASSWORD

                CommonEditTextInputType.EMAIL -> InputType.TYPE_CLASS_TEXT or
                        InputType.TYPE_TEXT_VARIATION_EMAIL_ADDRESS
            }
        }

        private fun getFilters(model: CommonEditTextUi): Array<InputFilter> {
            return characterFilterListener?.let { filterListener ->
                arrayOf(
                    InputFilter.LengthFilter(model.maxSymbols),
                    InputFilter { source, _, _, dest, dstart, dend ->
                        val sb = StringBuilder(dest)
                        sb.replace(dstart, dend, source.toString())
                        val tempModel = model.copy(selectedValue = sb.toString())
                        source.filter { char ->
                            filterListener.invoke(tempModel, char)
                        }
                    }
                )
            } ?: arrayOf(InputFilter.LengthFilter(model.maxSymbols))
        }
    }
}
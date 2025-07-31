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
import com.digitall.eid.databinding.ListItemPhoneTextBinding
import com.digitall.eid.extensions.hideKeyboard
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.extensions.moveCursorToEnd
import com.digitall.eid.extensions.noLeadingTrailingSpaces
import com.digitall.eid.extensions.onDonePressed
import com.digitall.eid.extensions.setBackgroundDrawableResource
import com.digitall.eid.extensions.setFocusChangeListener
import com.digitall.eid.extensions.showKeyboard
import com.digitall.eid.models.list.CommonListElementAdapterMarker
import com.digitall.eid.models.list.CommonPhoneTextUi
import com.google.android.material.textfield.TextInputLayout.END_ICON_CLEAR_TEXT

class CommonPhoneTextDelegate : CommonInputDelegate() {

    var doneListener: ((model: CommonPhoneTextUi) -> Unit)? = null
    var changeListener: ((model: CommonPhoneTextUi) -> Unit)? = null
    var focusChangedListener: ((model: CommonPhoneTextUi) -> Unit)? = null
    var characterFilterListener: ((model: CommonPhoneTextUi, char: Char) -> Boolean)? = null

    override fun isForViewType(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int
    ): Boolean {
        return items[position] is CommonPhoneTextUi
    }

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemPhoneTextBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position] as CommonPhoneTextUi)
    }

    private inner class ViewHolder(
        private val binding: ListItemPhoneTextBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        private var currentItem: CommonPhoneTextUi? = null

        private var textWatcher: TextWatcher? = null

        fun bind(model: CommonPhoneTextUi) {
            currentItem = model
            textWatcher?.let { binding.etValue.removeTextChangedListener(it) }
            binding.tvTitle.text = model.title.getString(binding.root.context)
            binding.icQuestion.isVisible = model.question
            binding.icRequired.isVisible = model.required
            binding.valueInputLayout.setEndIconStyle(
                mode = END_ICON_CLEAR_TEXT,
                drawable = R.drawable.ic_clean
            )
            model.countryCode?.let {
                binding.valueInputLayout.prefixText = it.getString(binding.root.context)
                binding.valueInputLayout.prefixTextView.updateLayoutParams {
                    height = ViewGroup.LayoutParams.MATCH_PARENT
                }
                binding.valueInputLayout.prefixTextView.gravity = Gravity.CENTER
            }
            model.countryCodeTextColor?.let {
                binding.valueInputLayout.setPrefixTextColor(
                    ColorStateList.valueOf(
                        binding.root.context.getColor(
                            it
                        )
                    )
                )
            }
            binding.valueInputLayout.hint = model.hint?.getString(binding.root.context)
            binding.etValue.setText(model.selectedValue)
            if (!model.selectedValue.isNullOrEmpty()) {
                binding.etValue.setBackgroundDrawableResource(R.drawable.bg_text)
                binding.valueInputLayout.setHintTextColorResource(R.color.color_0C53B2)
            } else {
                binding.etValue.setBackgroundDrawableResource(R.drawable.bg_text_not_entered)
                binding.valueInputLayout.setHintTextColorResource(R.color.color_94A3B8)
            }
            binding.etValue.setCompoundDrawablesWithIntrinsicBounds(R.drawable.transparent, 0, 0, 0)
            textWatcher = binding.etValue.doAfterTextChanged { editable ->
                if (!editable.isNullOrEmpty()) {
                    binding.etValue.setBackgroundDrawableResource(R.drawable.bg_text)
                    binding.valueInputLayout.setHintTextColorResource(R.color.color_0C53B2)
                } else {
                    binding.etValue.setBackgroundDrawableResource(R.drawable.bg_text_not_entered)
                    binding.valueInputLayout.setHintTextColorResource(R.color.color_94A3B8)
                }

                currentItem?.let { boundItem ->
                    if (isCurrentViewHolderVisible(bindingAdapterPosition = bindingAdapterPosition) && boundItem.hasFocus) {
                        changeListener?.invoke(boundItem.copy(selectedValue = editable.toString()))
                    }
                }
            }
            binding.etValue.isEnabled = model.isEnabled
            binding.etValue.filters = characterFilterListener?.let { filterListener ->
                arrayOf(
                    InputFilter.LengthFilter(model.maxSymbols),
                    InputFilter { source, _, _, dest, startEnd, _ ->
                        val selectedValue =
                            StringBuilder(dest.toString()).apply { insert(startEnd, source) }
                                .toString()
                        source.filter { char ->
                            filterListener.invoke(
                                model.copy(selectedValue = selectedValue),
                                char
                            )
                        }
                    })
            } ?: arrayOf(
                InputFilter.LengthFilter(model.maxSymbols)
            )
            binding.etValue.inputType = InputType.TYPE_CLASS_PHONE

            binding.etValue.onDonePressed {
                if (binding.etValue.text.isNullOrEmpty().not()) {
                    val trimmedText = binding.etValue.text?.toString()?.noLeadingTrailingSpaces()
                    binding.etValue.setText(trimmedText)
                    binding.etValue.setSelection(trimmedText?.length ?: 0)
                }
                doneListener?.invoke(
                    model.copy(
                        selectedValue = binding.etValue.text.toString()
                    )
                )
            }
            binding.etValue.setFocusChangeListener { hasFocus ->
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
                focusChangedListener?.invoke(
                    model.copy(
                        hasFocus = hasFocus,
                        selectedValue = binding.etValue.text.toString()
                    )
                )
            }

            binding.tvError.isVisible = model.validationError != null
            binding.tvError.text = model.validationError?.getString(binding.root.context)
        }
    }
}
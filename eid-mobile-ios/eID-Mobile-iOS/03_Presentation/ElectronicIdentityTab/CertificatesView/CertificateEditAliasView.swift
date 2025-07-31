//
//  CertificateEditAliasView.swift
//  eID-Mobile-iOS
//
//  Created by Steliyan Hadzhidenev on 12.03.25.
//

import SwiftUI

struct CertificateEditAliasView: View {
    
    // MARK: - Properties
    @Binding var originalAlias: String
    @StateObject var viewModel: CertificateEditAliasViewModel
    @Environment(\.presentationMode) var presentationMode
    
    // MARK: - Private Properties
    private let padding: CGFloat = 32
    
    // MARK: - Body
    var body: some View {
        VStack(spacing: padding) {
            aliasField
                .padding([.bottom], padding)
            buttons
            Spacer()
        }
        .contentShape(Rectangle())
        .padding()
        .addNavigationBar(title: "certificate_edit_alias_title".localized(),
                          content: {
            ToolbarItem(placement: .topBarLeading) {
                Button(action: {
                    presentationMode.wrappedValue.dismiss()
                }, label: {
                    Image("icon_arrow_left")
                })
            }
        })
        .observeLoading(isLoading: $viewModel.showLoading)
        .presentAlert(showAlert: $viewModel.showError,
                      alertText: $viewModel.errorText)
        .presentAlert(showAlert: $viewModel.showSuccess,
                      alertText: $viewModel.successText,
                      onDismiss: {
            originalAlias = viewModel.alias.value
            presentationMode.wrappedValue.dismiss()
        })
        .background(Color.themeSecondaryLight)
        .onTapGesture {
            hideKeyboard()
        }
    }
    
    // MARK: - Child views
    private var aliasField: some View {
        EIDInputField(title: "change_alias_title".localized(),
                      text: $viewModel.alias.value.max(30),
                      showError: !$viewModel.alias.validation.isValid,
                      errorText: $viewModel.alias.validation.error,
                      isMandatory: true,
                      keyboardType: .default,
                      submitLabel: .next,
                      autocapitalization: .never)
    }
    
    
    private var buttons: some View {
        Button(action: {
            viewModel.setAlias()
        }, label: {
            Text("btn_confirm")
        })
        .buttonStyle(EIDButton())
    }
}

#Preview {
    CertificateEditAliasView(
        originalAlias: .constant(""),
        viewModel: CertificateEditAliasViewModel(
            certificateId: "",
            alias: Alias(value: "Tet", original: "Test")
        )
    )
}

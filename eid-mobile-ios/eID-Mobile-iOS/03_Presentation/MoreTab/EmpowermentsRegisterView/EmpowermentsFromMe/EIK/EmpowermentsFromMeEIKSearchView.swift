//
//  EmpowermentsFromMeEIKView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 22.05.24.
//

import SwiftUI

struct EmpowermentsFromMeEIKSearchView: View {
    @StateObject var viewModel = EmpowermentsFromMeEIKSearchViewModel()
    @Environment(\.presentationMode) var presentationMode
    @Binding var path: [String]
    
    var body: some View {
        VStack(spacing: 24) {
            VStack {
                EIDInputField(title: "empowerment_eik_search_title".localized(),
                              text: $viewModel.eik.value,
                              showError: !$viewModel.eik.validation.isValid,
                              errorText: $viewModel.eik.validation.error,
                              isMandatory: true,
                              keyboardType: .numberPad)
                .padding([.bottom], 32)
                buttons
                Spacer()
            }
            .padding()
        }
        .frame(maxWidth: .infinity, maxHeight: .infinity)
        .background(Color.themeSecondaryLight)
        .addNavigationBar(title: "empowerments_eik_search_title".localized(),
                          content: {
            ToolbarItem(placement: .topBarLeading, content: {
                Button(action: {
                    presentationMode.wrappedValue.dismiss()
                }, label: {
                    Image("icon_arrow_left")
                })
            })
        })
        .observeLoading(isLoading: $viewModel.showLoading)
        .presentAlert(showAlert: $viewModel.showError,
                      alertText: $viewModel.errorText,
                      onDismiss: { presentationMode.wrappedValue.dismiss() })
        .onTapGesture {
            hideKeyboard()
        }
        .navigationDestination(isPresented: $viewModel.showEmpowermentsFromMeView) {
            EmpowermentsFromMeView(eik: viewModel.eik.value,
                                   path: $path)
        }
    }
    
    private var buttons: some View {
        Button(action: {
            if viewModel.validateFields() {
                viewModel.showEmpowermentsFromMeView = true
            }
        }, label: {
            Text("btn_empowerments_eik_search")
        })
        .buttonStyle(EIDButton())
    }
}

#Preview {
    EmpowermentsFromMeEIKSearchView(path: .constant([""]))
}

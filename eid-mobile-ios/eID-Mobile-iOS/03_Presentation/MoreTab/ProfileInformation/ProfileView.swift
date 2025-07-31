//
//  ProfileView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 18.06.24.
//

import SwiftUI


struct ProfileView: View {
    // MARK: - Properties
    @StateObject var viewModel = ProfileViewModel()
    @Environment(\.presentationMode) var presentationMode
    // MARK: - Private Properties
    private var padding: CGFloat = 32
    
    // MARK: - Body
    var body: some View {
        ScrollView {
            VStack(spacing: padding) {
                section(title: "application_section_personal_info_title".localized().uppercased())
                detailRow(
                    title: "first_name_title".localized(),
                    detail: viewModel.citizenEID?.firstName ?? "",
                    action: Action(icon: "icon_edit", color: .textDefault, action: viewModel.goToChangeInformation)
                )
                detailRow(
                    title: "second_name_title".localized(),
                    detail: viewModel.citizenEID?.secondName ?? "",
                    action: Action(icon: "icon_edit", color: .textDefault, action: viewModel.goToChangeInformation)
                )
                detailRow(
                    title: "last_name_title".localized(),
                    detail: viewModel.citizenEID?.lastName ?? "",
                    action: Action(icon: "icon_edit", color: .textDefault, action: viewModel.goToChangeInformation)
                )
                detailRow(
                    title: "first_name_latin_title".localized(),
                    detail: viewModel.citizenEID?.firstNameLatin ?? "",
                    action: Action(icon: "icon_edit", color: .textDefault, action: viewModel.goToChangeInformation)
                )
                detailRow(
                    title: "second_name_latin_title".localized(),
                    detail: viewModel.citizenEID?.secondNameLatin ?? "",
                    action: Action(icon: "icon_edit", color: .textDefault, action: viewModel.goToChangeInformation)
                )
                detailRow(
                    title: "last_name_latin_title".localized(),
                    detail: viewModel.citizenEID?.lastNameLatin ?? "",
                    action: Action(icon: "icon_edit", color: .textDefault, action: viewModel.goToChangeInformation)
                )
                section(title: "profile_section_contact_info_title".localized().uppercased())
                detailRow(
                    title: "phone_title".localized(),
                    detail: viewModel.citizenEID?.phoneNumber ?? "",
                    action: Action(icon: "icon_edit", color: .textDefault, action: viewModel.goToChangeInformation)
                )
                detailRow(
                    title: "application_details_email_title".localized(),
                    detail: viewModel.citizenEID?.email ?? "",
                    action: Action(icon: "icon_edit", color: .textDefault, action: viewModel.goToChangeEmail)
                )
                section(title: "profile_section_contact_eid_info_title".localized().uppercased())
                if let eidentityId = viewModel.citizenEID?.eidentityId {
                    detailRow(title: "eid_number_title".localized(), detail: eidentityId)
                }
                changePasswordButton
                Spacer()
            }
            .padding([.leading, .trailing, .top], padding)
        }
        .onAppear {
            viewModel.getCitizenEID()
        }
        .addNavigationBar(title: MoreMenuOption.profile.localizedTitle(),
                          content: {
            ToolbarItem(placement: .topBarLeading) {
                Button(action: {
                    presentationMode.wrappedValue.dismiss()
                }, label: {
                    Image("icon_arrow_left")
                })
            }
        })
        .navigationDestination(isPresented: $viewModel.handleAction, destination: {
            switch viewModel.selectedAction {
            case .changeInformation:
                ChangeInformationView(viewModel: ChangeInformationViewModel(citizenEid: viewModel.citizenEID))
            case .chanhgePassword:
                ChangePasswordView()
            case .changeEmail:
                ChangeEmailView()
            default:
                EmptyView()
            }
        })
        .background(Color.themeSecondaryLight)
        .observeLoading(isLoading: $viewModel.showLoading)
        .presentAlert(showAlert: $viewModel.showError,
                      alertText: $viewModel.errorText)
        .presentAlert(showAlert: $viewModel.showSuccess,
                      alertText: $viewModel.successText,
                      onDismiss: {
            viewModel.getCitizenEID()
        })
    }
    
    //MARK: Helpers
    private func detailRow(title: String, detail: String, action: Action? = nil) -> some View {
        VStack {
            Text(title)
                .font(.heading4)
                .lineSpacing(4)
                .foregroundStyle(Color.themeSecondaryDark)
                .frame(maxWidth: .infinity, alignment: .leading)
                .padding([.bottom], 8)
            HStack {
                Text(detail)
                    .font(.bodyRegular)
                    .foregroundStyle(Color.textDark)
                    .lineSpacing(8)
                    .frame(maxWidth: .infinity, alignment: .leading)
                if let action {
                    Spacer()
                    Button(
                        action: action.action,
                        label:{
                            Image(action.icon)
                                .renderingMode(.template)
                                .foregroundStyle(action.color)
                        })
                }
            }
        }
    }
    
    private func section(title: String) -> some View {
        VStack {
            Text(title)
                .font(.heading6)
                .lineSpacing(10)
                .foregroundStyle(Color.textDefault)
                .frame(maxWidth: .infinity, alignment: .leading)
            GradientDivider()
        }
    }
    
    private var changePasswordButton: some View {
        Button(
            action: viewModel.goToChangePassword,
            label: {
                Text("change_password_button_title".localized())
            }
        )
        .buttonStyle(EIDButton(buttonState: .primary))
    }
}

#Preview {
    ProfileView()
}

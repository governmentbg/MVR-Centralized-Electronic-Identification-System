//
//  ApplicationDetailsView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 7.03.24.
//

import SwiftUI

struct ApplicationDetailsView: View {
    // MARK: - Properties
    @StateObject var viewModel: ApplicationDetailsViewModel
    @Environment(\.presentationMode) var presentationMode
    @Binding var path: [String]
    
    // MARK: - Body
    var body: some View {
        VStack(spacing: 0) {
            titleView
            GradientDivider()
            ScrollView {
                VStack(spacing: 16) {
                    applicationDetailsView
                    GradientDivider()
                    citizenDetailsView
                    Spacer()
                    if viewModel.status == .pendingPayment {
                        paymentButton
                    }
                    if viewModel.displayContinueButton {
                        continueButton
                    }
                    backButton
                }
                .padding()
            }
        }
        .navigationDestination(isPresented: $viewModel.handleAction) {
            switch viewModel.selectedAction {
            case .goToCertificateDetails:
                if let details = viewModel.certificateDetails,
                   let history = viewModel.certificateHistory {
                    CertificateDetailsView(
                        viewModel: CertificateDetailsViewModel(
                            certificateDetails: details,
                            certificateHistory: history,
                            administrators: viewModel.administrators,
                            devices: viewModel.devices,
                            reasons: viewModel.reasons),
                        path: $path)
                }
            case .continueIssue:
                CompleteEIDApplicationView(viewModel:
                                            CompleteEIDApplicationViewModel(applicationId: viewModel.applicationId),
                                           path: $path)
            case .noAction:
                EmptyView()
            }
        }
        .frame(maxWidth: .infinity, maxHeight: .infinity)
        .background(Color.themeSecondaryLight)
        .addNavigationBar(title: "application_screen_title".localized(),
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
                      alertText: $viewModel.errorText)
        .presentAlert(showAlert: $viewModel.showSuccess,
                      alertText: $viewModel.successText,
                      onDismiss: {
            path.removeAll()
        })
    }
    
    // MARK: - Child views
    private var titleView: some View {
        Text(viewModel.applicationType?.screenTitle.localized() ?? "")
            .font(.heading4)
            .lineSpacing(8)
            .foregroundStyle(Color.textDefault)
            .padding()
    }
    
    private var applicationDetailsView: some View {
        VStack(alignment: .leading, spacing: 16) {
            Text("application_details_title".localized())
                .font(.bodySmallBold)
                .lineSpacing(8)
                .foregroundStyle(Color.textDefault)
            applicationNumberView
            if viewModel.certificateNumber != nil {
                certificateSerialNumberView
            }
            createDateView
            administratorView
            officeView
            carrierView
            statusView
            if viewModel.status == .pendingPayment {
                paymentAccessCodeView
            }
            if viewModel.reason != nil {
                reasonView
            }
        }
    }
    
    private var citizenDetailsView: some View {
        VStack(alignment: .leading, spacing: 16) {
            Text("application_section_personal_details_title".localized())
                .font(.bodySmallBold)
                .lineSpacing(10)
                .foregroundStyle(Color.textDefault)
            nameView
            emailView
            phoneNumberView
            identityTypeView
            identityNumberView
            identityIssueDateView
            identityValidToDateView
        }
        .padding()
        .background(RoundedRectangle(cornerRadius: 8)
            .fill(Color.themePrimaryLight))
    }
    
    /// Application details
    private var applicationNumberView: some View {
        DetailsViewRow(title: "application_number_title".localized(),
                       values: [viewModel.applicationNumber])
    }
    
    private var certificateSerialNumberView: some View {
        Button(action: {
            viewModel.getCertificateDetails()
        }, label: {
            DetailsViewRow(title: "certificate_serial_number_title".localized(),
                           specialContent: AnyView(
                            HStack {
                                Text(viewModel.certificateNumber ?? "")
                                    .font(.bodyLarge)
                                    .lineSpacing(8)
                                    .foregroundStyle(Color.textActive)
                                Image("icon_link")
                            }),
                           values: [])
        })
    }
    
    private var createDateView: some View {
        DetailsViewRow(title: "application_created_on_title".localized(),
                       values: [viewModel.createDateString])
    }
    
    private var administratorView: some View {
        DetailsViewRow(title: "application_details_administrator_title".localized(),
                       values: [viewModel.administratorName])
    }
    
    private var officeView: some View {
        DetailsViewRow(title: "application_details_administrator_office_title".localized(),
                       values: [viewModel.administratorOffice])
    }
    
    private var carrierView: some View {
        DetailsViewRow(title: "application_carrier_title".localized(),
                       values: [viewModel.deviceName])
    }
    
    private var statusView: some View {
        DetailsViewRow(title: "application_status_title".localized(),
                       specialContent: AnyView(
                        HStack {
                            Image(viewModel.status?.iconName ?? "")
                            Text(viewModel.status?.title.localized() ?? "")
                                .font(.bodyRegular)
                                .lineSpacing(8)
                                .foregroundStyle(viewModel.status?.textColor ?? .textDefault)
                        }),
                       values: [])
    }
    
    private var paymentAccessCodeView: some View {
        DetailsViewRow(title: "application_payment_access_code_title".localized(),
                       values: [viewModel.paymentAccessCode])
    }
    
    private var reasonView: some View {
        DetailsViewRow(title: "application_details_reason_title".localized(),
                       values: [viewModel.reason ?? ""])
    }
    
    /// Citizen details
    private var nameView: some View {
        DetailsViewRow(title: "application_details_name_title".localized(),
                       values: [viewModel.name])
    }
    
    private var emailView: some View {
        DetailsViewRow(title: "application_details_email_title".localized(),
                       values: [viewModel.email])
    }
    
    private var phoneNumberView: some View {
        DetailsViewRow(title: "application_details_phone_number_title".localized(),
                       values: [viewModel.phoneNumber])
    }
    
    private var identityTypeView: some View {
        DetailsViewRow(title: "document_type_title".localized(),
                       values: [viewModel.identityType])
    }
    
    private var identityNumberView: some View {
        DetailsViewRow(title: "document_number_title".localized(),
                       values: [viewModel.identityNumber])
    }
    
    private var identityIssueDateView: some View {
        DetailsViewRow(title: "identity_issue_date_title".localized(),
                       values: [viewModel.identityIssueDate])
    }
    
    private var identityValidToDateView: some View {
        DetailsViewRow(title: "identity_valid_to_date_title".localized(),
                       values: [viewModel.identityValidityToDate])
    }
    
    /// Buttons
    private var backButton: some View {
        Button(action: {
            presentationMode.wrappedValue.dismiss()
        }, label: {
            Text("btn_back".localized())
        })
        .buttonStyle(EIDButton(buttonType: .outline))
    }
    
    private var paymentButton: some View {
        Button(action: {
            PaymentHelper.openPaymentView(paymentAccessCode: viewModel.paymentAccessCode)
        }, label: {
            Text("btn_payment".localized())
        })
        .buttonStyle(EIDButton(buttonType: .filled))
    }
    
    private var continueButton: some View {
        Button(action: {
            switch viewModel.applicationType {
            case .resume, .revoke, .stop:
                viewModel.completeApplicationStatusChange()
            default:
                viewModel.handleAction = true
                viewModel.selectedAction = .continueIssue
            }
        }, label: {
            Text(viewModel.applicationType?.buttonTitle.localized() ?? "")
        })
        .buttonStyle(EIDButton(buttonType: .filled))
    }
}


// MARK: - Preview
#Preview {
    ApplicationDetailsView(viewModel: ApplicationDetailsViewModel(
        applicationDetails: ApplicationDetailsResponse(
            id: "",
            firstName: "",
            secondName: "",
            lastName: "",
            applicationType: .issue,
            status: .approved,
            createDate: "",
            deviceId: "",
            eidAdministratorName: "",
            eidAdministratorOfficeName: "",
            xml: "",
            submissionType: .eid,
            identityNumber: "",
            identityType: "",
            identityIssueDate: "",
            identityValidityToDate: "",
            applicationNumber: ""),
        administrators: [],
        devices: [],
        reasons: []), path: .constant([""]))
}

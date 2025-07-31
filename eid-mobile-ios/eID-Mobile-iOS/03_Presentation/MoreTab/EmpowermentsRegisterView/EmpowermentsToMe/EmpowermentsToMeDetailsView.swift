//
//  EmpowermentsToMeDetailsView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 20.11.23.
//

import SwiftUI


struct EmpowermentsToMeDetailsView: View {
    // MARK: - Properties
    @StateObject var viewModel: EmpowermentsToMeDetailsViewModel
    @Environment(\.presentationMode) var presentationMode
    
    // MARK: - Body
    var body: some View {
        VStack(spacing: 0) {
            titleView
            GradientDivider()
            ScrollView {
                VStack(spacing: 16) {
                    empowermentDetails
                    EmpowermentHistory(empowerment: viewModel.empowerment)
                    empowermentButtons
                }
                .padding()
            }
        }
        .frame(maxWidth: .infinity, maxHeight: .infinity)
        .background(Color.themeSecondaryLight)
        .addNavigationBar(title: "empowerments_to_me_screen_title".localized(),
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
                      onDismiss: { presentationMode.wrappedValue.dismiss() })
        .onTapGesture {
            hideKeyboard()
        }
    }
    
    // MARK: - Child views
    private var titleView: some View {
        Text(viewModel.screenTitle)
            .font(.heading4)
            .lineSpacing(8)
            .foregroundStyle(Color.textDefault)
            .padding()
    }
    
    private var empowermentDetails: some View {
        VStack(alignment: .leading, spacing: 16) {
            Text("empowerment_details_title".localized())
                .font(.heading6)
                .lineSpacing(10)
                .foregroundStyle(Color.themeSecondaryDark)
            authorizerDetails
            serviceDetails
        }
    }
    
    private var authorizerDetails: some View {
        VStack(alignment: .leading, spacing: 16) {
            DetailsViewRow(title: "empowerments_number_title".localized(),
                           values: [viewModel.empowerment.number ?? ""])
            DetailsViewRow(title: "empowerments_status_title".localized(),
                           specialContent: AnyView(
                            HStack {
                                Image(viewModel.empowerment.calculatedStatusOn?.iconName ?? "")
                                    .renderingMode(.template)
                                    .foregroundStyle(viewModel.empowerment.calculatedStatusOn?.textColor ?? .textError)
                                    .font(.bodyRegular)
                                Text(viewModel.empowerment.calculatedStatusOn?.title.localized() ?? "")
                                    .font(.bodyRegular)
                                    .lineSpacing(8)
                                    .foregroundStyle(viewModel.empowerment.calculatedStatusOn?.textColor ?? .textError)
                            }),
                           values: viewModel.empowerment.statusReason != nil ? [viewModel.empowerment.statusReason ?? ""] : [])
            DetailsViewRow(title: "empowerments_from_me_on_behalf_of_title".localized(),
                           values: [viewModel.empowerment.onBehalfOf.title.localized()])
            DetailsViewRow(title: "empowerment_start_date_title".localized()+":",
                           values: [viewModel.empowerment.startDate?.toDate()?.toLocalTime().normalizeDate(outputFormat: .iso8601DateTime) ?? ""])
            DetailsViewRow(title: "empowerment_end_date_title".localized()+":",
                           values: [viewModel.empowerment.expiryDate?.toDate()?.toLocalTime().normalizeDate(outputFormat: .iso8601DateTime) ?? "empowerment_expiry_date_indefinitely".localized()])
            DetailsViewRow(title: "empowerment_from_id_title".localized(),
                           values: [viewModel.empowerment.authorizerUids.first?.displayValue ?? ""])
            if viewModel.empowerment.onBehalfOf == .legalEntity {
                legalEntityDetails
            } else {
                DetailsViewRow(title: "empowerment_from_name_title".localized(),
                               values: [viewModel.empowerment.name ?? ""])
            }
            empoweredIdsDetails
        }
    }
    
    private var empoweredIdsDetails: some View {
        VStack(alignment: .leading, spacing: 16) {
            DetailsViewRow(title: "empowerment_to_ids_title".localized(),
                           values: viewModel.empowerment.empoweredUids.map({ $0.displayValue }))
            if viewModel.empowerment.empoweredUids.count > 1 {
                DetailsViewRow(title: "empowerment_type_field_title".localized(),
                               specialContent: AnyView(empowermentTypeDisclaimer),
                               values: [])
            }
        }
    }
    
    private var empowermentTypeDisclaimer: some View {
        VStack(alignment: .leading, spacing: 4) {
            Button(action: {}, label: {
                HStack {
                    Image("icon_warning")
                    Text(EmpowermentType.togetherOnly.title.localized())
                }
            })
            .buttonStyle(EIDButton(size: .small,
                                   buttonState: .warning,
                                   wide: false))
            Text("empowerment_type_together_only_description".localized())
                .font(.tiny)
                .lineSpacing(4)
                .foregroundStyle(Color.textDefault)
                .multilineTextAlignment(.leading)
        }
    }
    
    private var legalEntityDetails: some View {
        VStack(alignment: .leading, spacing: 16) {
            DetailsViewRow(title: "empowerments_from_me_on_behalf_of_title".localized(),
                           values: [viewModel.empowerment.onBehalfOf.title.localized()])
            DetailsViewRow(title: "empowerment_from_role_title".localized(),
                           values: [viewModel.empowerment.issuerPosition ?? ""])
            DetailsViewRow(title: "empowerment_from_bulstat_title".localized(),
                           values: [viewModel.empowerment.uid ?? ""])
            DetailsViewRow(title: "empowerment_from_legal_entity_name_title".localized(),
                           values: [viewModel.empowerment.name ?? ""])
        }
    }
    
    private var serviceDetails: some View {
        VStack(alignment: .leading, spacing: 16) {
            DetailsViewRow(title: "empowerment_provider_title".localized(),
                           values: [viewModel.empowerment.providerName ?? ""])
            DetailsViewRow(title: "empowerment_service_title".localized(),
                           values: [viewModel.empowerment.serviceName ?? ""])
            DetailsViewRow(title: "empowerment_scope_title".localized(),
                           values: [viewModel.empowerment.volumeOfRepresentationDescription])
        }
    }
    
    private var empowermentButtons: some View {
        VStack {
            if viewModel.viewState == .declareDisagreement {
                confirmDisagreementButton
            }
            backButton
        }
    }
    
    private var confirmDisagreementButton: some View {
        Button(action: {
            viewModel.declareDisagreement()
        }, label: {
            Text("empowerment_menu_option_declare_disagreement".localized())
        })
        .buttonStyle(EIDButton(buttonType: .filled,
                               buttonState: .danger))
    }
    
    private var backButton: some View {
        Button(action: {
            presentationMode.wrappedValue.dismiss()
        }, label: {
            Text("btn_back".localized())
        })
        .buttonStyle(EIDButton(buttonType: .outline))
    }
}

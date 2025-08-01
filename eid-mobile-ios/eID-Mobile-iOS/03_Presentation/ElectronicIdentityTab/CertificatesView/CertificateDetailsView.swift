//
//  CertificateDetailsView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 14.04.24.
//

import SwiftUI


struct CertificateDetailsView: View {
    // MARK: - Properties
    @StateObject var viewModel: CertificateDetailsViewModel
    @Environment(\.presentationMode) var presentationMode
    @Binding var path: [String]
    
    // MARK: - Body
    var body: some View {
        VStack(spacing: 0) {
            titleView
            GradientDivider()
            ScrollView {
                VStack(spacing: 16) {
                    serialNumberView
                    aliasView
                    eidentityIdView
                    commonNameView
                    statusView
                    if viewModel.shouldShowReasonView {
                        reasonView
                    }
                    administratorView
                    deviceView
                    levelOfAssuranceView
                    validityFromView
                    validityUntilView
                    if viewModel.viewState == .preview {
                        historyView
                    }
                    Spacer()
                    if viewModel.viewState != .preview {
                        actionButton
                    }
                    backButton
                }
                .padding()
            }
        }
        .frame(maxWidth: .infinity, maxHeight: .infinity)
        .background(Color.themeSecondaryLight)
        .addNavigationBar(title: "certificate_screen_title".localized(),
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
                      onDismiss: {
            presentationMode.wrappedValue.dismiss()
        })
    }
    
    // MARK: - Child views
    private var titleView: some View {
        Text("certificate_preview_title".localized())
            .font(.heading4)
            .lineSpacing(8)
            .foregroundStyle(Color.textDefault)
            .multilineTextAlignment(.center)
            .padding()
    }
    
    private var reasonView: some View {
        DetailsViewRow(title: viewModel.reasonViewTitle,
                       values: [viewModel.reason])
    }
    
    private var eidentityIdView: some View {
        DetailsViewRow(title: "certificate_eidentity_id_title".localized(),
                       values: [viewModel.eidentityId])
    }
    
    private var aliasView: some View {
        DetailsViewRow(
            title: "certificate_alias_title".localized(),
            action: Action(
                icon: "icon_edit",
                color: .textDefault,
                action: { viewModel.goToSetAlias() }
            ),
            values: [viewModel.certitifaceAlias]
        )
        .id(UUID())
    }
    
    private var serialNumberView: some View {
        DetailsViewRow(title: "certificate_serial_number_title".localized(),
                       values: [viewModel.serialNumber])
    }
    
    private var commonNameView: some View {
        DetailsViewRow(title: "certificate_common_name_title".localized(),
                       values: [viewModel.commonName])
    }
    
    private var statusView: some View {
        DetailsViewRow(title: "certificate_status_title".localized(),
                       specialContent: AnyView(
                        statusRow(iconName: viewModel.status?.iconName,
                                  title: viewModel.status?.title,
                                  color: viewModel.status?.textColor)),
                       values: [])
    }
    
    private var expiringView: some View {
        HStack {
            Text("certificate_item_expires_soon_title".localized())
                .padding(2)
                .font(.extraTiny)
                .foregroundStyle(Color.textDefault)
                .textCase(.uppercase)
                .background(
                    RoundedRectangle(cornerRadius: 4,
                                     style: .continuous)
                    .fill(Color.buttonDangerHover)
                )
            Spacer()
        }
    }
    
    private var validityFromView: some View {
        DetailsViewRow(title: "certificate_valid_from_title".localized(),
                       values: [viewModel.validityFrom])
    }
    
    private var validityUntilView: some View {
        DetailsViewRow(title: "certificate_valid_until_title".localized(),
                       specialContent: viewModel.isExpiring ? AnyView(expiringView) : nil,
                       values: [viewModel.validityUntil],
                       specialContentOnBottom: true)
    }
    
    private var administratorView: some View {
        DetailsViewRow(title: "certificate_details_administrator_title".localized(),
                       values: [viewModel.eidAdministratorName])
    }
    
    private var deviceView: some View {
        DetailsViewRow(title: "certificate_carrier_title".localized(),
                       values: [viewModel.deviceName])
    }
    
    private var levelOfAssuranceView: some View {
        DetailsViewRow(title: "certificate_level_of_assurance_title".localized(),
                       values: [viewModel.levelOfAssurance?.title.localized() ?? ""])
    }
    
    private var historyView: some View {
        VStack(alignment: .leading, spacing: 16) {
            Text("certificate_history_title".localized())
                .font(.heading6Bold)
                .lineSpacing(10)
                .foregroundStyle(Color.themeSecondaryDark)
            ForEach(viewModel.certificateHistoryItems, id: \.self) { event in
                Divider()
                    .foregroundStyle(Color.themePrimaryDark)
                DetailsViewRow(title: "certificate_status_title".localized(),
                               specialContent: AnyView(
                                statusRow(iconName: event.status?.iconName,
                                          title: event.status?.title,
                                          color: event.status?.textColor)),
                               values: [])
                if event.hasApplicationNumber,
                   let applicationId = event.applicationId,
                   let applicationNumber = event.applicationNumber {
                    DetailsViewRow(title: "certificate_history_application_created_on_title".localized(),
                                   values: [event.createdDateTime.toDate()?.toLocalTime().normalizeDate(outputFormat: .iso8601DateTime) ?? ""])
                    Button(action: {
                        viewModel.getApplicationDetails(forId: applicationId)
                    }, label: {
                        DetailsViewRow(title: "certificate_application_number_title".localized(),
                                       specialContent: AnyView(
                                        HStack {
                                            Text(applicationNumber)
                                                .font(.bodyLarge)
                                                .lineSpacing(8)
                                                .foregroundStyle(Color.textActive)
                                            Image("icon_link")
                                        }),
                                       values: [])
                    })
                }
                DetailsViewRow(title: "certificate_status_application_modified_date_title".localized(),
                               values: [event.modifiedDateTime?.toDate()?.toLocalTime().normalizeDate(outputFormat: .iso8601DateTime) ?? ""])
                if event.status == .created {
                    DetailsViewRow(title: "certificate_valid_from_title".localized(),
                                   values: [event.validityFrom.toDate()?.toLocalTime().normalizeDate(outputFormat: .iso8601DateTime) ?? ""])
                    DetailsViewRow(title: "certificate_valid_until_title".localized(),
                                   values: [event.validityUntil.toDate()?.toLocalTime().normalizeDate(outputFormat: .iso8601DateTime) ?? ""])
                }
                if let reason = event.getReason(fromReasons: viewModel.reasons) {
                    DetailsViewRow(title: "empowerment_reason_title".localized(),
                                   values: [reason])
                }
            }
        }
        .padding()
        .navigationDestination(isPresented: $viewModel.handleAction) {
            switch viewModel.selectedAction {
            case .goToApplicationDetails:
                if let details = viewModel.applicationDetails {
                    ApplicationDetailsView(
                        viewModel: ApplicationDetailsViewModel(
                            applicationDetails: details,
                            administrators: viewModel.administrators,
                            devices: viewModel.devices,
                            reasons: viewModel.reasons
                        ), path: $path
                    )
                }
            case .goToSetAlias:
                NavigationView {
                    let id = viewModel.id
                    let currentAlias = viewModel.certitifaceAlias
                    CertificateEditAliasView(
                        originalAlias: $viewModel.certitifaceAlias,
                        viewModel: CertificateEditAliasViewModel(
                            certificateId: id,
                            alias: Alias(
                                value: currentAlias,
                                original: currentAlias
                            )
                        )
                    )
                }
                .toolbar(.hidden)
            case .noAction:
                EmptyView()
            }
        }
        .background(RoundedRectangle(cornerRadius: 8)
            .fill(Color.themePrimaryLight))
    }
    
    private var actionButton: some View {
        NavigationLink(destination: CreateApplicationView(
            certificateActionsInfo: viewModel.certificateInfo,
            viewState: viewModel.viewState,
            reasons: viewModel.certificateStatusChangeReasons,
            viewModel: CreateApplicationViewModel(administrators: viewModel.administrators,
                                                  devices: viewModel.devices),
            path: $path)) {
                HStack {
                    Image(viewModel.buttonConfig.icon)
                        .renderingMode(.template)
                        .foregroundColor(.white)
                    Text(viewModel.buttonConfig.title.localized())
                        .foregroundColor(.white)
                }
            }
            .id(UUID())
            .buttonStyle(EIDButton(buttonType: .filled,
                                   buttonState: viewModel.buttonState))
    }
    
    private var backButton: some View {
        Button(action: {
            presentationMode.wrappedValue.dismiss()
        }, label: {
            Text("btn_back".localized())
        })
        .buttonStyle(EIDButton(buttonType: .outline))
    }
    
    private func statusRow(iconName: String?, title: String?, color: Color?) -> some View {
        HStack {
            Image(iconName ?? "")
                .renderingMode(.template)
                .resizable()
                .frame(width: 25, height: 25)
                .foregroundColor(color ?? .buttonDefault)
            Text(title?.localized() ?? "")
                .font(.bodyRegular)
                .lineSpacing(8)
                .foregroundStyle(color ?? .textError)
        }
    }
}


// MARK: - Preview
#Preview {
    CertificateDetailsView(viewModel: CertificateDetailsViewModel(
        certificateDetails:
            CertificateDetailsResponse(
                id: "id-123",
                applicationNumber: "123",
                status: .active,
                eidAdministratorId: "",
                eidAdministratorOfficeId: "",
                eidAdministratorName: "MVR",
                eidentityId: "eid-id-123",
                commonName: "Удостоверение ",
                validityFrom: "",
                validityUntil: "",
                createDate: "",
                serialNumber: "",
                deviceId: ""),
        certificateHistory: [],
        state: .preview,
        administrators: [],
        devices: [],
        reasons: []), path: .constant([""]))
}

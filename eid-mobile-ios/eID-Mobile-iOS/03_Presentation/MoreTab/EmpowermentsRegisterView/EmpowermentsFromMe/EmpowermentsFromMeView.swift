//
//  EmpowermentsFromMeView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 8.11.23.
//

import SwiftUI


struct EmpowermentsFromMeView: View {
    // MARK: - Properties
    @StateObject var viewModel = EmpowermentsFromMeViewModel()
    var eik: String?
    @State private var showOptions = false
    @Binding var path: [String]
    
    // MARK: - Body
    var body: some View {
        EmpowermentsBaseView(title: viewModel.screenTitle,
                             empowermentDirection: eik == nil ? .fromMe : .fromMeEIK,
                             viewModel: viewModel,
                             menuView: floatingButtonMenu,
                             content: listView)
        .onAppear {
            if let eik = eik {
                viewModel.viewState = .fromMeEIK
                viewModel.eik = eik
            }
            viewModel.getEmpowerments()
        }
    }
    
    // MARK: - Child views
    private var listView: some View {
        ScrollView {
            if viewModel.empowerments.count == 0 && !viewModel.showLoading {
                EmptyListView()
            }
            LazyVStack {
                ForEach(viewModel.empowerments, id: \.self) { empowerment in
                    empowermentRow(empowerment: empowerment)
                }
                Text("")
                    .onAppear {
                        viewModel.scrollViewBottomReached()
                    }
            }
        }
        .refreshable {
            viewModel.getEmpowerments()
        }
        .navigationDestination(isPresented: $viewModel.handleAction) {
            switch viewModel.selectedAction {
            case .sign, .withdraw:
                if let empowerment = viewModel.targetEmpowerment {
                    EmpowermentsFromMeDetailsView(viewModel:
                                                    EmpowermentsFromMeDetailsViewModel(
                                                        empowerment: empowerment,
                                                        viewState: viewModel.selectedAction == .sign ? .sign : .withdraw))
                }
            default:
                NavigationView {
                    switch viewModel.selectedAction {
                    case .copy:
                        CreateEmpowermentView(viewModel: CreateEmpowermentViewModel(
                            empowement: viewModel.targetEmpowerment),
                                              path: $path)
                    case .create:
                        CreateEmpowermentView(viewModel: CreateEmpowermentViewModel(),
                                              path: $path)
                    case .eikSearch:
                        EmpowermentsFromMeEIKSearchView(path: $path)
                    default: EmptyView()
                    }
                }
                .navigationBarHidden(true)
            }
        }
    }
    
    private var floatingButtonMenu: some View {
        Button(action: {
            showOptions.toggle()
        }, label: {
            Image("icon_vertical_dots")
                .renderingMode(.template)
                .foregroundColor(.white)
                .padding(12)
                .background(Color.buttonConfirm)
                .clipShape(RoundedRectangle(cornerRadius: 40))
                .addItemShadow()
        })
        .autoEdgePopover(isPresented: $showOptions, content: {
            menuOptions
                .presentationCompactAdaptation(.popover)
        })
        .addItemShadow()
        .padding(16)
    }
    
    private var menuOptions: some View {
        let actions = [EmpowermentAction.create, EmpowermentAction.eikSearch]
        return VStack(spacing: 0) {
            ForEach(actions, id: \.self) { action in
                Button(action: {
                    showOptions = false
                    viewModel.handleEmpowermentAction(action, for: nil)
                }, label: {
                    MenuOptionItem(icon: action.icon,
                                   text: action.title.localized(),
                                   textColor: action.textColor,
                                   iconColor: action.textColor,
                                   padding: 16,
                                   alignment: .leading)
                })
                
                if action != actions.last {
                    GradientDivider()
                }
            }
        }
    }
    
    private func empowermentRow(empowerment: Empowerment) -> some View {
        NavigationLink(destination: EmpowermentsFromMeDetailsView(viewModel: EmpowermentsFromMeDetailsViewModel(
            empowerment: empowerment,
            viewState: ((empowerment.status == .collectingAuthorizerSignatures
                         || empowerment.status == .awaitingSignature) && empowerment.shouldSign)
            ? .sign
            : .preview)),
                       label: {
            EmpowermentFromMeItem(number: empowerment.number ?? "",
                                  providerName: empowerment.providerName ?? "",
                                  authorizerName: empowerment.name ?? "",
                                  serviceName: empowerment.serviceName ?? "",
                                  authorizedId: empowerment.empoweredUids.first?.displayValue ?? "",
                                  authorizedIdsCount: empowerment.empoweredUids.count,
                                  createdOn: empowerment.createdOn?.toDate()?.toLocalTime().normalizeDate(outputFormat: .iso8601DateTime) ?? "",
                                  shouldSign: empowerment.shouldSign,
                                  status: empowerment.calculatedStatusOn,
                                  signAction: { viewModel.handleEmpowermentAction(.sign, for: empowerment) },
                                  copyAction: { viewModel.handleEmpowermentAction(.copy, for: empowerment) },
                                  withdrawAction: { viewModel.handleEmpowermentAction(.withdraw, for: empowerment) })
        })
        .addItemShadow()
        .onAppear {
            if empowerment.id == viewModel.empowerments.last?.id {
                viewModel.scrollViewBottomReached()
            }
        }
    }
}

// MARK: - Preview
#Preview {
    EmpowermentsFromMeView(eik: "361094227",
                           path: .constant([""]))
}

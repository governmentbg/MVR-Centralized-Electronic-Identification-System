//
//  EmpowermentsToMeView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 20.11.23.
//

import SwiftUI


struct EmpowermentsToMeView: View {
    // MARK: - Properties
    @StateObject var viewModel = EmpowermentsToMeViewModel()
    @State private var hideEmpowermentsItemOptions: Bool = false
    
    // MARK: - Body
    var body: some View {
        EmpowermentsBaseView(title: "empowerments_to_me_screen_title".localized(),
                             empowermentDirection: .toMe,
                             viewModel: viewModel,
                             content: listView)
        .onAppear {
            hideEmpowermentsItemOptions = false
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
            }
        }
        .refreshable {
            viewModel.getEmpowerments()
        }
        .navigationDestination(isPresented: $viewModel.handleAction) {
            if viewModel.selectedAction == .declareDisagreement {
                EmpowermentsToMeDetailsView(viewModel:
                                                EmpowermentsToMeDetailsViewModel(
                                                    empowerment: viewModel.targetEmpowerment!,
                                                    viewState: .declareDisagreement))
            }
        }
    }
    
    private func empowermentRow(empowerment: Empowerment) -> some View {
        NavigationLink(destination: EmpowermentsToMeDetailsView(viewModel: EmpowermentsToMeDetailsViewModel(empowerment: empowerment))
            .onAppear {
                hideEmpowermentsItemOptions = true
            },
                       label: {
            EmpowermentToMeItem(empowermentNumber: empowerment.number ?? "",
                                providerName: empowerment.providerName ?? "",
                                authorizerName: empowerment.name ?? "",
                                serviceName: empowerment.serviceName ?? "",
                                createdOn: empowerment.createdOn?.toDate()?.toLocalTime().normalizeDate(outputFormat: .iso8601DateTime) ?? "",
                                status: empowerment.calculatedStatusOn,
                                hideItemMenuOptions: $hideEmpowermentsItemOptions,
                                declareDisagreementAction: {
                viewModel.handleEmpowermentAction(.declareDisagreement, for: empowerment)
            })
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
    EmpowermentsToMeView()
}

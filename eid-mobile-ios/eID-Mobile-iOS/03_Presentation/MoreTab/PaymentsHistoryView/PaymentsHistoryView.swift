//
//  PaymentsHistoryView.swift
//  eID-Mobile-iOS
//
//  Created by Steliyan Hadzhidenev on 17.03.25.
//

import SwiftUI

struct PaymentsHistoryView: View {
    // MARK: - Properties
    @StateObject var viewModel = PaymentsHistoryViewModel()
    @Environment(\.presentationMode) var presentationMode
    
    // MARK: - Body
    var body: some View {
        ZStack(alignment: .bottomTrailing) {
            VStack(spacing: 0) {
                controlsView
                ScrollView {
                    if viewModel.filteredPayments.count == 0 && !viewModel.showLoading {
                        EmptyListView()
                    }
                    LazyVStack {
                        ForEach(viewModel.filteredPayments, id: \.self) { paymentHistory in
                            paymentHistoryRow(paymentHistory: paymentHistory)
                        }
                    }
                }
                .refreshable {
                    viewModel.getPayments()
                }
            }
            if viewModel.showFilter {
                filterView
            }
        }
        .frame(maxWidth: .infinity, maxHeight: .infinity)
        .background(Color.themeSecondaryLight)
        .addNavigationBar(title: "payments_history_screen_title".localized(),
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
        .onAppear {
            viewModel.getPayments()
        }
    }
    
    // MARK: Child views
    private var controlsView: some View {
        HStack {
            sortView
            Spacer()
            EIDFilterButton(
                isFilterApplied: $viewModel.isFilterApplied,
                action: { viewModel.showFilter = true }
            )
        }
        .padding()
    }
    
    private var sortView: some View {
        Menu(content: {
            Button(action: {
                viewModel.sortCriteria = nil
                viewModel.sortDirection = nil
                viewModel.applySortingAndFilter()
            }, label: {
                HStack {
                    Text("sort_by_default_title".localized())
                    if viewModel.sortCriteria == nil && viewModel.sortDirection == nil {
                        Image("icon_selected_blue")
                    }
                }
            })
            ForEach(PaymentHistorySortCriteria.allCases, id: \.self) { criteria in
                Button(action: {
                    viewModel.sortCriteria = criteria
                    viewModel.sortDirection = .asc
                    viewModel.applySortingAndFilter()
                }, label: {
                    HStack {
                        Text("\(criteria.title.localized())  (\(SortDirection.asc.title.localized()))")
                        if viewModel.sortCriteria == criteria && viewModel.sortDirection == .asc {
                            Image("icon_selected_blue")
                        }
                    }
                })
                Button(action: {
                    viewModel.sortCriteria = criteria
                    viewModel.sortDirection = .desc
                    viewModel.applySortingAndFilter()
                }, label: {
                    HStack {
                        Text("\(criteria.title.localized())  (\(SortDirection.desc.title.localized()))")
                        if viewModel.sortCriteria == criteria && viewModel.sortDirection == .desc {
                            Image("icon_selected_blue")
                        }
                    }
                })
            }
        }, label: {
            HStack(spacing: 0) {
                Text(String(format: "sort_by_title".localized(), viewModel.sortTitle))
                    .font(.tiny)
                    .lineSpacing(4)
                    .multilineTextAlignment(.leading)
                    .foregroundStyle(Color.textLight)
                Image("icon_arrow_down_small")
            }
        })
        .preferredColorScheme(.light)
    }
    
    private var filterView: some View {
        PaymentsHistoryFilterView(
            paymentNumber: $viewModel.paymentNumber,
            status: $viewModel.status,
            createdOnDateStr: $viewModel.createdOn,
            reason: $viewModel.reason,
            amount: $viewModel.amount,
            paymentDateDateStr: $viewModel.paymentDate,
            validUntilDateStr: $viewModel.validUntil,
            applyFilter: {
                viewModel.showFilter = false
                viewModel.applySortingAndFilter()
            },
            dismiss: { viewModel.showFilter = false })
    }
    
    private func paymentHistoryRow(paymentHistory: PaymentHistoryResponse) -> some View {
        PaymentHistoryItem(
            paymentNumber: paymentHistory.ePaymentId ?? "",
            createdOn: paymentHistory.createdOn ?? "",
            subject: paymentHistory.reason ?? PaymentHistoryReason.issueEid,
            paymentDate: paymentHistory.paymentDate ?? "",
            status: paymentHistory.status ?? PaymentHistoryStatus.paid,
            validUntil: paymentHistory.paymentDeadline ?? "",
            payment: paymentHistory.payment ?? [],
            lastUpdated: paymentHistory.lastSync ?? ""
        )
        .addItemShadow()
    }
}

#Preview {
    PaymentsHistoryView()
}

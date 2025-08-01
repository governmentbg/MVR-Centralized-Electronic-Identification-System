//
//  EmpowermentsBaseView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 22.05.24.
//

import SwiftUI


struct EmpowermentsBaseView<Content: View, MenuView: View>: View { 
    // MARK: - Properties 
    @StateObject var viewModel: EmpowermentsBaseViewModel 
    @Environment(\.presentationMode) var presentationMode
    @ViewBuilder let content: Content
    @ViewBuilder let menuView: MenuView?
    var eik: String = ""
    var title: String = ""
    var empowermentDirection: EmpowermentDirection
    
    // MARK: - Body
    init(title: String,
         empowermentDirection: EmpowermentDirection,
         viewModel: EmpowermentsBaseViewModel,
         eik: String? = nil,
         menuView: MenuView? = EmptyView(),
         content: Content) {
        _viewModel = StateObject(wrappedValue: viewModel)
        self.title = title
        self.empowermentDirection = empowermentDirection
        self.eik = eik ?? ""
        self.menuView = menuView
        self.content = content
    }
    
    var body: some View {
        ZStack(alignment: .bottomTrailing) {
            VStack(spacing: 0) {
                controlsView
                content
            }
            if let menuView = menuView {
                menuView
            }
            if viewModel.showFilter {
                filterView
            }
        }
        .frame(maxWidth: .infinity, maxHeight: .infinity)
        .background(Color.themeSecondaryLight)
        .addNavigationBar(title: title,
                          content: {
            ToolbarItem(placement: .topBarLeading, content: {
                Button(action: {
                    if viewModel.showFilter {
                        viewModel.showFilter = false
                    } else {
                        presentationMode.wrappedValue.dismiss()
                    }
                }, label: {
                    Image("icon_arrow_left")
                })
            })
        })
        .observeLoading(isLoading: $viewModel.showLoading)
        .presentAlert(showAlert: $viewModel.showError,
                      alertText: $viewModel.errorText,
                      onDismiss: { presentationMode.wrappedValue.dismiss() }) 
    }
    
    // MARK: - Child views
    var controlsView: some View {
        HStack {
            sortView
            Spacer()
            EIDFilterButton(isFilterApplied: $viewModel.isFilterApplied,
                            action: { viewModel.showFilter = true })
        }
        .padding()
    }
    
    var sortView: some View {
        Menu(content: {
            Button(action: {
                viewModel.sortCriteria = nil
                viewModel.sortDirection = nil
                viewModel.reloadDataFromStart()
            }, label: {
                HStack {
                    Text("sort_by_default_title".localized())
                    if viewModel.sortCriteria == nil && viewModel.sortDirection == nil {
                        Image("icon_selected_blue")
                    }
                }
            })
            ForEach(empowermentDirection == .fromMe
                    ? EmpowermentSortCriteria.fromMeCriteria
                    : EmpowermentSortCriteria.toMeCriteria, id: \.self) { criteria in
                Button(action: {
                    viewModel.sortCriteria = criteria
                    viewModel.sortDirection = .asc
                    viewModel.reloadDataFromStart()
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
                    viewModel.reloadDataFromStart()
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
    
    var filterView: some View {
        EmpowermentsFilterView(empowermentDirection: empowermentDirection,
                               number: $viewModel.number,
                               status: $viewModel.status,
                               onBehalfOf: $viewModel.onBehalfOf,
                               authorizer: $viewModel.authorizer,
                               providerName: $viewModel.providerName,
                               serviceName: $viewModel.serviceName,
                               showOnlyNoExpiryDate: $viewModel.showOnlyNoExpiryDate,
                               validToDateText: $viewModel.validToDate,
                               empoweredUids: $viewModel.empoweredUids,
                               eik: $viewModel.eik,
                               applyFilter: {
            viewModel.showFilter = false
            viewModel.reloadDataFromStart()
        },
                               dismiss: {
            viewModel.showFilter = false
        })
    }
}

#Preview {
    EmpowermentsBaseView(title: "",
                         empowermentDirection: .fromMe, 
                         viewModel: EmpowermentsBaseViewModel(),
                         content: Text(""))
}

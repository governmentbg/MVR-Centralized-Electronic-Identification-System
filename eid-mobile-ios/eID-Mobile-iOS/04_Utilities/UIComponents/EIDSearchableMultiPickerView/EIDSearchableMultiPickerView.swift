//
//  EIDSearchableMultiPickerView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 2.11.23.
//

import SwiftUI


struct MultiPickerViewItem: Identifiable, Hashable {
    var id: String
    var name: String
    var selected: Bool
}


struct EIDSearchableMultiPickerView: View {
    // MARK: - Properties
    @Environment(\.presentationMode) var presentationMode
    @State var searchText: String = ""
    @State var title: String
    @Binding var items: [MultiPickerViewItem]
    @Binding var allSelected: Bool
    @State var showAllSelectedToggle: Bool = true
    var onSelectionChange: (() -> Void)? = nil
    var itemsToDisplay: [MultiPickerViewItem] {
        return searchText.isEmpty ? items : items.filter { $0.name.caseInsensitiveContains(searchText) }
    }
    
    // MARK: - Body
    var body: some View {
        NavigationStack {
            VStack(alignment: .leading) {
                if itemsToDisplay.count == 0 {
                    noResultsText
                }
                if showAllSelectedToggle {
                    if searchText.isEmpty {
                        Toggle(isOn: $allSelected) {
                            Text("title_select_all".localized())
                                .font(.bodyRegular)
                                .lineSpacing(8)
                                .foregroundStyle(Color.textLight)
                        }
                        .toggleStyle(CheckboxToggleStyle(action: { toggleAll() }))
                    }
                }
                List(items.indices, id: \.self) { index in
                    if searchText.isEmpty || items[index].name.caseInsensitiveContains(searchText) {
                        Toggle(isOn: $items[index].selected){
                            Text(items[index].name)
                                .font(.bodyRegular)
                                .lineSpacing(8)
                                .foregroundStyle(Color.textLight)
                        }
                        .toggleStyle(CheckboxToggleStyle(action: {
                            onSelectionChange?()
                            if items.filter({ !$0.selected }).isEmpty {
                                allSelected = true
                            } else {
                                allSelected = false
                            }
                        }))
                        .listRowBackground(Color.backgroundWhite)
                        .listRowSeparator(.hidden)
                    }
                }
                .listStyle(.plain)
                .background(Color.backgroundWhite)
            }
            .padding()
            .frame(maxWidth: .infinity, maxHeight: .infinity)
            .background(Color.backgroundWhite)
            .addNavigationBar(title: title,
                              content: {
                ToolbarItem(placement: .topBarTrailing, content: {
                    Button(action: {
                        presentationMode.wrappedValue.dismiss()
                    }, label: {
                        Image("icon_cross")
                            .padding([.top, .bottom, .leading])
                    })
                })
            })
        }
        .searchable(text: $searchText,
                    placement: .navigationBarDrawer(displayMode: .always))
        .tint(Color.textWhite)
        .onAppear {
            UITextField.appearance(whenContainedInInstancesOf: [UISearchBar.self]).backgroundColor = .backgroundWhite
            UITextField.appearance(whenContainedInInstancesOf: [UISearchBar.self]).tintColor = .textDefault
            UILabel.appearance(whenContainedInInstancesOf: [UISearchBar.self]).textColor = .textDefault
            UISearchBar.appearance().setImage(searchBarImage(), for: .search, state: .normal)
        }
        .environment(\.colorScheme, .light)
    }
    
    // MARK: - Helpers
    private func toggleAll() {
        if items.filter({ !$0.selected }).isEmpty {
            for (index, _) in items.enumerated() {
                items[index].selected = false
            }
        } else {
            for (index, _) in items.enumerated() {
                items[index].selected = true
            }
        }
        onSelectionChange?()
    }
    
    private var noResultsText: some View {
        Text("empty_search_result_title".localized())
            .font(.bodyRegular)
            .lineSpacing(8)
            .foregroundStyle(Color.textLight)
            .padding()
    }
    
    private func searchBarImage() -> UIImage {
        let image = UIImage(systemName: "magnifyingglass")
        return image?.withTintColor(.textDefault, renderingMode: .alwaysOriginal) ?? UIImage()
    }
}


// MARK: - Preview
#Preview {
    EIDSearchableMultiPickerView(title: "Picker",
                                 items: .constant([MultiPickerViewItem(id: "0",
                                                                       name: "Zero",
                                                                       selected: false),
                                                   MultiPickerViewItem(id: "1",
                                                                       name: "One",
                                                                       selected: false),
                                                   MultiPickerViewItem(id: "2",
                                                                       name: "Two",
                                                                       selected: false)]),
                                 allSelected: .constant(false))
}

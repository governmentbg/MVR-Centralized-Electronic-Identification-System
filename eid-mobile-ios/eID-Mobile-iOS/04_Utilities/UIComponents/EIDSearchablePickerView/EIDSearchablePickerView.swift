//
//  EIDSearchablePickerView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 2.11.23.
//

import SwiftUI


struct PickerViewItem: Identifiable, Hashable {
    var id: String
    var name: String
}


struct EIDSearchablePickerView: View {
    // MARK: - Properties
    @Environment(\.presentationMode) var presentationMode
    @State var searchText: String = ""
    @State var title: String
    @State var items: [PickerViewItem]
    @Binding var selection: String
    @State var didSelectItem: ((PickerViewItem) -> Void)? = nil
    var itemsToDisplay: [PickerViewItem] {
        return searchText.isEmpty ? items : items.filter { $0.name.caseInsensitiveContains(searchText) }
    }
    
    // MARK: - Body
    var body: some View {
        NavigationStack {
            VStack(alignment: .leading) {
                if itemsToDisplay.count == 0 {
                    noResultsText
                }
                List(itemsToDisplay, id: \.self) { item in
                    Button(action: {
                        selection = item.id
                        didSelectItem?(item)
                        presentationMode.wrappedValue.dismiss()
                    }, label: {
                        Text(item.name)
                            .font(.bodyRegular)
                            .lineSpacing(8)
                            .foregroundStyle(Color.textLight)
                    })
                    .listRowBackground(Color.backgroundWhite)
                    .listRowSeparator(.hidden)
                }
                .listStyle(.plain)
                .background(Color.backgroundWhite)
            }
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
        }
    }
    
    // MARK: - Child views
    private var noResultsText: some View {
        Text("empty_search_result_title".localized())
            .font(.bodyRegular)
            .lineSpacing(8)
            .foregroundStyle(Color.textLight)
            .padding()
    }
}


// MARK: - Preview
#Preview {
    EIDSearchablePickerView(title: "Picker",
                            items: [PickerViewItem(id: "0", 
                                                   name: "Zero"),
                                    PickerViewItem(id: "1", 
                                                   name: "One"),
                                    PickerViewItem(id: "2", 
                                                   name: "Two")],
                            selection: .constant(""))
}

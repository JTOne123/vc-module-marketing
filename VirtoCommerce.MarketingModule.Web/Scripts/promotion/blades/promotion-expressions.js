﻿angular.module('virtoCommerce.marketingModule')
.controller('promotionExpressionsController', ['$scope', 'settings', 'bladeNavigationService', function ($scope, settings, bladeNavigationService) {

    
    $scope.openItemSelectWizard = function (parentElement) {
        var selectedListEntries = [];
        var newBlade = {
            id: "CatalogEntrySelect",
            title: "Pick Product for promotion condition",
            controller: 'catalogItemSelectController',
            template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/common/catalog-items-select.tpl.html',
            breadcrumbs: [],
            bladeToolbarCommands: [
            {
                name: "Pick selected", icon: 'fa fa-plus',
                executeMethod: function (blade) {
                    //parentElement.selectedListEntry = selectedListEntries[0];
                    parentElement.productId = selectedListEntries[0].id;
                    bladeNavigationService.closeBlade(blade);
                },
                canExecuteMethod: function () {
                    return selectedListEntries.length == 1;
                }
            }]
        };

        newBlade.options = {
            allowMultiple: true,
            checkItemFn: function (listItem, isSelected) {
                if (listItem.type == 'category') {
                    newBlade.error = 'Must select Product';
                    listItem.selected = undefined;
                } else {
                    if (isSelected) {
                        if (_.all(selectedListEntries, function (x) { return x.id != listItem.id; })) {
                            selectedListEntries.push(listItem);
                        }
                    }
                    else {
                        selectedListEntries = _.reject(selectedListEntries, function (x) { return x.id == listItem.id; });
                    }
                    newBlade.error = undefined;
                }
            }
        };

        bladeNavigationService.showBlade(newBlade, $scope.blade);
    }

    $scope.openCategorySelectWizard = function (parentElement) {
        var selectedListEntries = [];
        var newBlade = {
            id: "CatalogCategorySelect",
            title: "Pick Category for promotion condition",
            controller: 'catalogItemSelectController',
            template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/common/catalog-items-select.tpl.html',
            breadcrumbs: [],
            bladeToolbarCommands: [
            {
                name: "Pick selected", icon: 'fa fa-plus',
                executeMethod: function (blade) {
                    parentElement.selectedCategoryId = selectedListEntries[0].id;
                    bladeNavigationService.closeBlade(blade);
                },
                canExecuteMethod: function () {
                    return selectedListEntries.length == 1;
                }
            }]
        };

        newBlade.options = {
            allowMultiple: true,
            checkItemFn: function (listItem, isSelected) {
                if (listItem.type != 'category') {
                    newBlade.error = 'Must select Category';
                    listItem.selected = undefined;
                } else {
                    if (isSelected) {
                        if (_.all(selectedListEntries, function (x) { return x.id != listItem.id; })) {
                            selectedListEntries.push(listItem);
                        }
                    }
                    else {
                        selectedListEntries = _.reject(selectedListEntries, function (x) { return x.id == listItem.id; });
                    }
                    newBlade.error = undefined;
                }
            }
        };

        bladeNavigationService.showBlade(newBlade, $scope.blade);
    }

    $scope.availableCurrencies = settings.getValues({ id: 'VirtoCommerce.Core.General.Currencies' });
}]);
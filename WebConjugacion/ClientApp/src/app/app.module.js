"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.AppModule = void 0;
var platform_browser_1 = require("@angular/platform-browser");
var core_1 = require("@angular/core");
var forms_1 = require("@angular/forms");
var http_1 = require("@angular/common/http");
var router_1 = require("@angular/router");
var animations_1 = require("@angular/platform-browser/animations");
var toolbar_1 = require("@angular/material/toolbar");
var list_1 = require("@angular/material/list");
var grid_list_1 = require("@angular/material/grid-list");
var card_1 = require("@angular/material/card");
var button_1 = require("@angular/material/button");
var dialog_1 = require("@angular/material/dialog");
var form_field_1 = require("@angular/material/form-field");
var input_1 = require("@angular/material/input");
var checkbox_1 = require("@angular/material/checkbox");
var select_1 = require("@angular/material/select");
var slide_toggle_1 = require("@angular/material/slide-toggle");
var progress_spinner_1 = require("@angular/material/progress-spinner");
var material_1 = require("@angular/material");
var tree_1 = require("@angular/material/tree");
var icon_1 = require("@angular/material/icon");
var verbs_service_1 = require("./services/verbs.service");
var tenses_service_1 = require("./services/tenses.service");
var app_component_1 = require("./app.component");
var nav_menu_component_1 = require("./nav-menu/nav-menu.component");
var word_check_component_1 = require("./word-check/word-check.component");
var verbs_tree_component_1 = require("./verbs-tree/verbs-tree.component");
var AppModule = /** @class */ (function () {
    function AppModule() {
    }
    AppModule = __decorate([
        core_1.NgModule({
            declarations: [
                app_component_1.AppComponent,
                nav_menu_component_1.NavMenuComponent,
                word_check_component_1.WordCheckComponent,
                verbs_tree_component_1.VerbsTreeComponent
            ],
            imports: [
                platform_browser_1.BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
                http_1.HttpClientModule,
                forms_1.FormsModule,
                router_1.RouterModule.forRoot([
                    { path: '', redirectTo: '/word-check', pathMatch: 'full' },
                    { path: 'word-check', component: word_check_component_1.WordCheckComponent },
                    { path: 'verbs-tree', component: verbs_tree_component_1.VerbsTreeComponent },
                ]),
                animations_1.BrowserAnimationsModule,
                toolbar_1.MatToolbarModule,
                list_1.MatListModule,
                grid_list_1.MatGridListModule,
                card_1.MatCardModule,
                button_1.MatButtonModule,
                dialog_1.MatDialogModule,
                input_1.MatInputModule,
                checkbox_1.MatCheckboxModule,
                form_field_1.MatFormFieldModule,
                select_1.MatSelectModule,
                slide_toggle_1.MatSlideToggleModule,
                progress_spinner_1.MatProgressSpinnerModule,
                material_1.MatSnackBarModule,
                tree_1.MatTreeModule,
                icon_1.MatIconModule
            ],
            providers: [verbs_service_1.VerbsService,
                tenses_service_1.TensesService,
            ],
            bootstrap: [app_component_1.AppComponent]
        })
    ], AppModule);
    return AppModule;
}());
exports.AppModule = AppModule;
//# sourceMappingURL=app.module.js.map
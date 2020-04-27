import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { VerbsTreeComponent } from './verbs-tree.component';

describe('VerbsTreeComponent', () => {
  let component: VerbsTreeComponent;
  let fixture: ComponentFixture<VerbsTreeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ VerbsTreeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(VerbsTreeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

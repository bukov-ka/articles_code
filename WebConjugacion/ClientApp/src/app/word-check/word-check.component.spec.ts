import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WordCheckComponent } from './word-check.component';

describe('WordCheckComponent', () => {
  let component: WordCheckComponent;
  let fixture: ComponentFixture<WordCheckComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WordCheckComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WordCheckComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

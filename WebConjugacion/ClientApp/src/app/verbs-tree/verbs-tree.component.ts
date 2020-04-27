import { Component, OnInit } from '@angular/core';
import { SelectionModel } from '@angular/cdk/collections';
import { FlatTreeControl } from '@angular/cdk/tree';
import { Injectable } from '@angular/core';
import { MatTreeFlatDataSource, MatTreeFlattener } from '@angular/material/tree';
import { BehaviorSubject } from 'rxjs';
import { Tense } from '../shared/models/tense';
import { Mood } from '../shared/models/mood';
import { TensesService } from '../services/tenses.service';


/**
 * Node for to-do item
 */
export class TenseItemNode {
  children: TenseItemNode[];
  item: string;
  key: string;
}

/** Flat to-do item node with expandable and level information */
export class TensItemFlatNode {
  item: string;
  key: string;
  level: number;
  expandable: boolean;
}

/**
 * Checklist database, it can build a tree structured Json object.
 * Each node in Json object represents a tense tree item or a category.
 * If a node is a category, it has children items and new items can be added under the category.
 */
@Injectable()
export class ChecklistDatabase {
  dataChange = new BehaviorSubject<TenseItemNode[]>([]);
  moods: Mood[];

  get data(): TenseItemNode[] { return this.dataChange.value; }

  constructor(private tensesService: TensesService) {
    this.initialize();
    var verbs = this.tensesService.getTenses();
    verbs.subscribe(s => {
      this.moods = s;
      const data = this.buildFileTree(this.moods, 0);
      this.dataChange.next(data);
    });
  }

  initialize() {
  }

  /**
   * Build the file structure tree. The `value` is the Json object, or a sub-tree of a Json object.
   * The return value is the list of `TenseItemNode`.
   */
  buildFileTree(obj: Mood[], level: number): TenseItemNode[] {
    return obj.reduce<TenseItemNode[]>((accumulator, currentIndex) => {
      const value = currentIndex;
      const node = new TenseItemNode();
      node.item = value.name;

      node.children = value.exact_tenses.map(m => {
        var child = new TenseItemNode();
        child.item = m.name;
        child.key = m.exact_name;
        return child;
      });

      return accumulator.concat(node);
    }, []);
  }
}



@Component({
  selector: 'app-verbs-tree',
  templateUrl: './verbs-tree.component.html',
  styleUrls: ['./verbs-tree.component.css'],
  providers: [ChecklistDatabase]
})
export class VerbsTreeComponent implements OnInit {

  /** Map from flat node to nested node. This helps us finding the nested node to be modified */
  flatNodeMap = new Map<TensItemFlatNode, TenseItemNode>();

  /** Map from nested node to flattened node. This helps us to keep the same object for selection */
  nestedNodeMap = new Map<TenseItemNode, TensItemFlatNode>();

  /** A selected parent node to be inserted */
  selectedParent: TensItemFlatNode | null = null;

  /** The new item's name */
  newItemName = '';

  treeControl: FlatTreeControl<TensItemFlatNode>;

  treeFlattener: MatTreeFlattener<TenseItemNode, TensItemFlatNode>;

  dataSource: MatTreeFlatDataSource<TenseItemNode, TensItemFlatNode>;

  /** The selection for checklist */
  checklistSelection = new SelectionModel<TensItemFlatNode>(true /* multiple */);

  constructor(private _database: ChecklistDatabase,
    private tensesService: TensesService) {
    this.treeFlattener = new MatTreeFlattener(this.transformer, this.getLevel,
      this.isExpandable, this.getChildren);
    this.treeControl = new FlatTreeControl<TensItemFlatNode>(this.getLevel, this.isExpandable);
    this.dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);

    _database.dataChange.subscribe(data => {
      this.dataSource.data = data;
      this.treeControl.expandAll();

      var selection = this.tensesService.filterTenses;
      if(!!selection)
        this.treeControl.dataNodes.forEach(node => {
          if (selection.indexOf(node.key) > -1) {
            this.checklistSelection.select(node);
          }
      })
    });    
  }



  ngOnInit() {
  }

  getLevel = (node: TensItemFlatNode) => node.level;

  isExpandable = (node: TensItemFlatNode) => node.expandable;

  getChildren = (node: TenseItemNode): TenseItemNode[] => node.children;

  hasChild = (_: number, _nodeData: TensItemFlatNode) => _nodeData.expandable;

  hasNoContent = (_: number, _nodeData: TensItemFlatNode) => _nodeData.item === '';

  /**
   * Transformer to convert nested node to flat node. Record the nodes in maps for later use.
   */
  transformer = (node: TenseItemNode, level: number) => {
    const existingNode = this.nestedNodeMap.get(node);
    const flatNode = existingNode && existingNode.item === node.item
      ? existingNode
      : new TensItemFlatNode();
    flatNode.item = node.item;
    flatNode.key = node.key;
    flatNode.level = level;
    flatNode.expandable = !!node.children;
    this.flatNodeMap.set(flatNode, node);
    this.nestedNodeMap.set(node, flatNode);
    return flatNode;
  }

  /** Whether all the descendants of the node are selected. */
  descendantsAllSelected(node: TensItemFlatNode): boolean {
    const descendants = this.treeControl.getDescendants(node);
    const descAllSelected = descendants.every(child =>
      this.checklistSelection.isSelected(child)
    );
    return descAllSelected;
  }

  /** Whether part of the descendants are selected */
  descendantsPartiallySelected(node: TensItemFlatNode): boolean {
    const descendants = this.treeControl.getDescendants(node);
    const result = descendants.some(child => this.checklistSelection.isSelected(child));
    return result && !this.descendantsAllSelected(node);
  }

  /** Toggle the item selection. Select/deselect all the descendants node */
  tenseItemSelectionToggle(node: TensItemFlatNode): void {
    this.checklistSelection.toggle(node);
    const descendants = this.treeControl.getDescendants(node);
    this.checklistSelection.isSelected(node)
      ? this.checklistSelection.select(...descendants)
      : this.checklistSelection.deselect(...descendants);

    // Force update for the parent
    descendants.every(child =>
      this.checklistSelection.isSelected(child)
    );
    this.checkAllParentsSelection(node);
  }

  /** Toggle a leaf item selection. Check all the parents to see if they changed */
  tenseLeafItemSelectionToggle(node: TensItemFlatNode): void {
    this.checklistSelection.toggle(node);
    this.checkAllParentsSelection(node);
  }

  /* Checks all the parents when a leaf node is selected/unselected */
  checkAllParentsSelection(node: TensItemFlatNode): void {
    let parent: TensItemFlatNode | null = this.getParentNode(node);
    while (parent) {
      this.checkRootNodeSelection(parent);
      parent = this.getParentNode(parent);
    }
    this.updateTenseFilter();
  }

  /** Check root node checked state and change it accordingly */
  checkRootNodeSelection(node: TensItemFlatNode): void {
    const nodeSelected = this.checklistSelection.isSelected(node);
    const descendants = this.treeControl.getDescendants(node);
    const descAllSelected = descendants.every(child =>
      this.checklistSelection.isSelected(child)
    );
    if (nodeSelected && !descAllSelected) {
      this.checklistSelection.deselect(node);
    } else if (!nodeSelected && descAllSelected) {
      this.checklistSelection.select(node);
    }
  }

  /* Get the parent node of a node */
  getParentNode(node: TensItemFlatNode): TensItemFlatNode | null {
    const currentLevel = this.getLevel(node);

    if (currentLevel < 1) {
      return null;
    }

    const startIndex = this.treeControl.dataNodes.indexOf(node) - 1;

    for (let i = startIndex; i >= 0; i--) {
      const currentNode = this.treeControl.dataNodes[i];

      if (this.getLevel(currentNode) < currentLevel) {
        return currentNode;
      }
    }
    return null;
  }

  updateTenseFilter() {
    var selectedTenses = this.checklistSelection.selected.filter(f => !!f.key).map(m => m.key);
    this.tensesService.filterTenses = selectedTenses;
  }
}

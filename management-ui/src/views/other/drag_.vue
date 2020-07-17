<template>
  <div>
    <a-row>
      <a-col :span="12">
        <draggable :list="list1" group="people" @change="log">
          <a-button
            v-for="(element, index) in list1"
            :key="element.name"
          >{{ element.name }} {{ index }}</a-button>
        </draggable>
      </a-col>
      <a-col :span="12">
        <draggable :list="list2" group="people" @change="log">
          <div
            class="list-group-item"
            v-for="(element, index) in list2"
            :key="element.name"
          >{{ element.name }} {{ index }}</div>
        </draggable>
      </a-col>
    </a-row>
  </div>
</template>
<script>
import draggable from 'vuedraggable'
export default {
  name: 'two-lists',
  display: 'Two Lists',
  order: 1,
  components: {
    draggable
  },
  data() {
    return {
      list1: [{ name: 'John', id: 1 }, { name: 'Joao', id: 2 }, { name: 'Jean', id: 3 }, { name: 'Gerard', id: 4 }],
      list2: [{ name: 'Juan', id: 5 }, { name: 'Edgard', id: 6 }, { name: 'Johnson', id: 7 }]
    }
  },
  methods: {
    add: function() {
      this.list.push({ name: 'Juan' })
    },
    replace: function() {
      this.list = [{ name: 'Edgard' }]
    },
    clone: function(el) {
      return {
        name: el.name + ' cloned'
      }
    },
    log: function(evt) {
      window.console.log(evt)
    }
  }
}
</script>
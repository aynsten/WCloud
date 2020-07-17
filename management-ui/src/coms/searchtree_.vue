<template>
  <div>
    <a-row>
      <a-col :span="24">
        <a-input-search
          size="small"
          style="margin-bottom: 8px;max-width:220px;"
          placeholder="搜索..."
          @change="search_tree_"
        >
          <a-button slot="enterButton" type="dashed" icon="search" :loading="loading"></a-button>
        </a-input-search>
        <a-tooltip placement="bottom" style="float:right;">
          <template slot="title">
            <span>展开全部</span>
          </template>
          <a-button
            type="dashed"
            icon="eye"
            size="small"
            @click="()=>{expand_keys=find_in_tree(tree_data,x=>{})}"
          ></a-button>
        </a-tooltip>
      </a-col>
    </a-row>
    <a-tree
      checkable
      v-model="checked_keys"
      :treeData="tree_data"
      :expandedKeys="expand_keys"
      @expand="(x)=>{expand_keys=x}"
      @check="(x)=>{this.$emit('check', x);}"
    >
      <template slot="title" slot-scope="item">
        <span v-if="item.match">
          <span style="color: red">{{item.title}}</span>
        </span>
        <span v-else>{{item.title}}</span>
      </template>
    </a-tree>
  </div>
</template>

<script>
import _ from 'lodash'
import { mixin } from '@/utils/mixin'

export default {
  props: {
    data: {
      type: Array,
      default: () => []
    },
    checked: {
      type: Array,
      default: () => []
    }
  },
  mixins: [mixin],
  data() {
    return {
      loading: false,
      tree_data: [],
      expand_keys: [],
      checked_keys: []
    }
  },
  created() {
    this.update_tree_data(this.data)
    this.update_checked_keys(this.checked)
  },
  methods: {
    update_tree_data(val) {
      var data_list = val || []
      var repeat = this.find_in_tree(data_list, node => {
        node.scopedSlots = { title: 'title' }
      })
      this.tree_data = data_list
    },
    update_checked_keys(val) {
      val = val || []
      var uids = this.find_in_tree(this.tree_data)
      //如果勾选key在树中不存在会报警告
      this.checked_keys = val.filter(x => uids.indexOf(x) >= 0)
    },
    search_tree_: _.debounce(function(e) {
      var _this = this
      _this.loading = true
      _this.expand_keys = []

      var q = e.target.value
      var repeat = []

      var __match_func__ = function(x, keyword) {
        return x.title.toLowerCase().indexOf(keyword.toLowerCase()) >= 0
      }

      var find = function(node) {
        if (repeat.indexOf(node.key) >= 0) {
          alert('树结构有问题')
          return false
        }
        repeat.push(node.key)
        //只输入一个不搜索
        if (q.length > 0) {
          //当前节点匹配
          var this_match = __match_func__(node, q)
          //子节点有匹配
          var children_match = node.children.map(x => find(x)).filter(x => x === true).length > 0
          if (children_match) {
            _this.expand_keys.push(node.key)
          }

          node.match = this_match

          return this_match || children_match
        } else {
          node.match = false
          return false
        }
      }

      _this.tree_data.forEach(item => {
        find(item)
      })

      setTimeout(() => {
        _this.loading = false
      }, 500)
    }, 500)
  },
  watch: {
    data: function(val) {
      this.update_tree_data(val)
      this.$nextTick(() => {
        this.update_checked_keys(this.checked)
      })
    },
    checked: function(val) {
      this.update_checked_keys(val)
    }
  }
}
</script>

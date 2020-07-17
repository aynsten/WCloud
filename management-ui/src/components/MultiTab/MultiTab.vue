
<template>
  <div style="margin: -23px -24px 24px -24px">
    <a-tabs
      hideAdd
      v-model="activeKey"
      :tabBarStyle="{ background: '#FFF', margin: 0, paddingLeft: '16px', paddingTop: '1px' }"
      size="small"
    >
      <a-tab-pane v-for="page in tagPages" :key="page.fullPath">
        <span slot="tab">
          <a-dropdown :trigger="['contextmenu']">
            <span>{{page.meta.title}}</span>
            <a-menu slot="overlay" @click="(x)=>close_tab(x,page)" theme="light">
              <a-menu-item key="refresh_this">
                <a-icon type="sync" />
                <span>刷新页面</span>
              </a-menu-item>
              <a-menu-divider />
              <a-menu-item key="close_this">
                <a-icon type="close-circle" />
                <span>关闭当前</span>
              </a-menu-item>
              <a-menu-item key="close_all">
                <a-icon type="stop" />
                <span>关闭其他</span>
              </a-menu-item>
            </a-menu>
          </a-dropdown>
        </span>
      </a-tab-pane>
    </a-tabs>
  </div>
</template>

<script>
export default {
  name: 'MultiTab',
  inject: ['reload'],
  data() {
    return {
      pages: [],
      activeKey: ''
    }
  },
  methods: {
    close_tab({ key }, page) {
      if (key === 'refresh_this') {
        this.reload()
      } else if (key === 'close_this') {
        this.closeThis(page.fullPath)
      } else if (key === 'close_all') {
        this.closeAll(page.fullPath)
      }
      console.log(key, page)
    },
    selectedLastPath() {
      if (this.fullPathList.length > 0) {
        this.activeKey = this.fullPathList[this.fullPathList.length - 1]
      }
    },
    closeThis(targetKey) {
      if (this.tagPages.length <= 1) {
        this.$message.info('最后一个页面')
        return
      }
      this.pages = this.pages.filter(page => page.fullPath !== targetKey)
      // 判断当前标签是否关闭，若关闭则跳转到最后一个还存在的标签页
      if (!this.fullPathList.includes(this.activeKey)) {
        this.selectedLastPath()
      }
    },
    closeAll(currentPath) {
      const currentIndex = this.fullPathList.indexOf(currentPath)
      this.fullPathList.forEach((item, index) => {
        if (index !== currentIndex) {
          this.closeThis(item)
        }
      })
    }
  },
  computed: {
    fullPathList() {
      return this.tagPages.map(x => x.fullPath)
    },
    tagPages() {
      return this.pages.filter(x => !(x.meta || {}).no_tab)
    }
  },
  created() {
    this.pages.push(this.$route)
    this.selectedLastPath()
  },
  watch: {
    $route: function(newVal) {
      this.activeKey = newVal.fullPath
      if (this.fullPathList.indexOf(newVal.fullPath) < 0) {
        this.pages.push(newVal)
      }
    },
    activeKey: function(newPathKey) {
      this.$router.push({ path: newPathKey })
    }
  }
}
</script>

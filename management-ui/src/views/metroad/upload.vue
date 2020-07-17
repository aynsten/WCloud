<template>
  <div>
    <a-spin :spinning="loading" tip="uploading">
      <a-upload
        listType="picture-card"
        accept="image/*"
        :fileList="fileList"
        :customRequest="upload_file"
        :remove="handleRemove"
        @preview="handlePreview"
      >
        <div v-if="fileList.length < max_count">
          <a-icon type="plus" />
          <div class="ant-upload-text">上传</div>
        </div>
      </a-upload>
    </a-spin>
    <a-modal v-model="previewVisible" :footer="false">
      <img alt="example" style="width: 100%" :src="previewImage" />
    </a-modal>
  </div>
</template>

<script>
import { post_form, post_form_data } from '@/api/ajax'
import { mixin } from '@/utils/mixin'

export default {
  mixins: [mixin],
  props: {
    image_list: {
      type: Array,
      default: () => [],
    },
    max_count: {
      type: Number,
      default: 9
    }
  },
  data() {
    return {
      loading: false,
      previewVisible: false,
      previewImage: '',
      fileList: []
    }
  },
  watch: {
    image_list(val) {
      this.set_image(val)
    }
  },
  mounted() {
    this.set_image(this.image_list)
  },
  methods: {
    set_image(data) {
      data = data || []
      var index = 0
      var res = data.map(x => ({
        uid: `uid${++index}`,
        name: `name${++index}`,
        status: 'done',
        url: x
      }))
      this.fileList = res
    },
    out_image() {
      var res = this.fileList.map(x => x.url)
      console.log('图片：', res)
      this.$emit('change', res)
    },
    handlePreview(file) {
      this.previewImage = file.url || file.thumbUrl
      this.previewVisible = true
    },
    handleRemove(file) {
      if (!confirm('确定移除此图片？')) {
        return
      }
      this.fileList = this.fileList.filter(x => x.uid !== file.uid)
      this.out_image()
    },
    upload_file(data) {
      this.loading = true
      var formData = new FormData()
      formData.append('file', data.file)
      post_form_data(this, '/api/metro-ad/admin/common/Common/Upload', formData)
        .then(res => {
          if (res.Success) {
            var upload_res = res.Data
            var img = {
              uid: upload_res.UID,
              name: upload_res.OriginFileName,
              status: 'done',
              url: upload_res.Url
            }
            if (!this.fileList.some(x => x.uid === img.uid)) {
              this.fileList.push(img)
              this.out_image()
            }
          } else {
            alert(res.ErrorMsg)
          }
        })
        .finally(() => {
          this.loading = false
        })
    }
  }
}
</script>